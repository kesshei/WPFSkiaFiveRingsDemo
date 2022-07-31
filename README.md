# SkiaSharp 之 WPF 自绘 五环弹动球（案例版）

>此案例基于拖曳和弹动球两个技术功能实现，如有不懂的可以参考之前的相关文章，属于递进式教程。

# 五环弹动球
好吧，名字是我起的，其实，你可以任意个球进行联动弹动，效果还是很不错的，有很多前端都是基于这个特效，可以搞出一些很有科技感的效果出来。

## Wpf 和 SkiaSharp
新建一个WPF项目，然后，Nuget包即可
要添加Nuget包
```csharp
Install-Package SkiaSharp.Views.WPF -Version 2.88.0
```
其中核心逻辑是这部分，会以我设置的60FPS来刷新当前的画板。
```csharp
skContainer.PaintSurface += SkContainer_PaintSurface;
_ = Task.Run(() =>
{
    while (true)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                skContainer.InvalidateVisual();
            });
            _ = SpinWait.SpinUntil(() => false, 1000 / 60);//每秒60帧
        }
        catch
        {
            break;
        }
    }
});
```
## 弹球实体代码 (Ball.cs)
```csharp
public class Ball
{
    public double X { get; set; }
    public double Y { get; set; }
    public double VX { get; set; }
    public double VY { get; set; }
    public int Radius { get; set; }
    public bool Dragged { get; set; } = false;
    public SKColor sKColor { get; set; } = SKColors.Blue;
    public bool CheckPoint(SKPoint sKPoint)
    {
        var d = Math.Sqrt(Math.Pow(sKPoint.X - X, 2) + Math.Pow(sKPoint.Y - Y, 2));
        return this.Radius >= d;
    }
}
```

## 五环弹动核心类 (FiveRings.cs)
```csharp
/// <summary>
/// 五环弹球
/// </summary>
public class FiveRings
{
    public SKPoint centerPoint;
    public int Radius = 0;
    public int BallLength = 8;

    public double TargetX;
    public double Spring = 0.03;
    public double SpringLength = 200;
    public double Friction = 0.95;
    public List<Ball>? Balls;
    public Ball? draggedBall;
    public void init(SKCanvas canvas, SKTypeface Font, int Width, int Height)
    {
        if (Balls == null)
        {
            Balls = new List<Ball>();
            for (int i = 0; i < BallLength; i++)
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                Balls.Add(new Ball()
                {
                    X = random.Next(50, Width - 50),
                    Y = random.Next(50, Height - 50),
                    Radius = this.Radius
                });
            }
        }
    }
    /// <summary>
    /// 渲染
    /// </summary>
    public void Render(SKCanvas canvas, SKTypeface Font, int Width, int Height)
    {
        centerPoint = new SKPoint(Width / 2, Height / 2);
        this.Radius = 20;
        this.TargetX = Width / 2;
        init(canvas, Font, Width, Height);
        canvas.Clear(SKColors.White);


        //划线
        using var LinePaint = new SKPaint
        {
            Color = SKColors.Green,
            Style = SKPaintStyle.Fill,
            StrokeWidth = 3,
            IsStroke = true,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };
        SKPath path = null;
        foreach (var item in Balls)
        {
            if (path == null)
            {
                path = new SKPath();
                path.MoveTo((float)item.X, (float)item.Y);
            }
            else
            {
                path.LineTo((float)item.X, (float)item.Y);
            }
        }
        path.Close();
        canvas.DrawPath(path, LinePaint);


        foreach (var item in Balls)
        {
            if (!item.Dragged)
            {
                foreach (var ball in Balls.Where(t => t != item).ToList())
                {
                    SpringTo(item, ball);
                }
            }
            DrawCircle(canvas, item);
        }

        using var paint = new SKPaint
        {
            Color = SKColors.Blue,
            IsAntialias = true,
            Typeface = Font,
            TextSize = 24
        };
        string by = $"by 蓝创精英团队";
        canvas.DrawText(by, 600, 400, paint);

    }
    /// <summary>
    /// 画一个圆
    /// </summary>
    public void DrawCircle(SKCanvas canvas, Ball ball)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Blue,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 2
        };
        canvas.DrawCircle((float)ball.X, (float)ball.Y, ball.Radius, paint);
    }
    public void MouseMove(SKPoint sKPoint)
    {
        if (draggedBall != null)
        {
            draggedBall.X = sKPoint.X;
            draggedBall.Y = sKPoint.Y;
        }
    }
    public void MouseDown(SKPoint sKPoint)
    {
        foreach (var item in Balls)
        {
            if (item.CheckPoint(sKPoint))
            {
                item.Dragged = true;
                draggedBall = item;
            }
            else
            {
                item.Dragged = false;
            }
        }
    }
    public void MouseUp(SKPoint sKPoint)
    {
        draggedBall = null;
        foreach (var item in Balls)
        {
            item.Dragged = false;
        }
    }
    public void SpringTo(Ball b1, Ball b2)
    {
        var dx = b2.X - b1.X;
        var dy = b2.Y - b1.Y;
        var angle = Math.Atan2(dy, dx);
        var targetX = b2.X - SpringLength * Math.Cos(angle);
        var targetY = b2.Y - SpringLength * Math.Sin(angle);

        b1.VX += (targetX - b1.X) * Spring;
        b1.VY += (targetY - b1.Y) * Spring;

        b1.VX *= Friction;
        b1.VY *= Friction;

        b1.X += b1.VX;
        b1.Y += b1.VY;
    }
}
```

## 效果如下:

![](https://tupian.wanmeisys.com/markdown/1659276325589-fc1f4de9-1fc4-4a4b-b2d6-d3e816e209ba.gif)


这个特效用的好，也能产生一些神奇的效果。

## 总结
这次是结合拖曳和弹动效果实现的综合案例，效果还是很不错的，之前也没想到原来还可以这样玩，拓展了玩法啊。

## 代码地址
https://github.com/kesshei/WPFSkiaFiveRingsDemo.git
 
https://gitee.com/kesshei/WPFSkiaFiveRingsDemo.git

# 阅
一键三连呦！，感谢大佬的支持，您的支持就是我的动力!

# 版权
蓝创精英团队（公众号同名，CSDN同名，CNBlogs同名）



