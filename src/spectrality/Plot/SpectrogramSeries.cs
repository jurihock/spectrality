using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;

class SpectrogramSeries : XYAxisSeries
{
    public Spectrogram? Spectrogram { get; private set; }
    public OxyImage? Image { get; private set; }

    public SpectrogramSeries(Spectrogram spectrogram)
    {
        Spectrogram = spectrogram;
        Image = new SpectrogramImage((-120, 0)).GetImage(spectrogram);
    }

    public override void Render(IRenderContext rc)
    {
        if (Spectrogram == null)
        {
            return;
        }

        Image ??= new SpectrogramImage((-120, 0)).GetImage(Spectrogram.Value);

        var freqs = Spectrogram.Value.Frequencies;
        var times = Spectrogram.Value.Timestamps;
        var data = Spectrogram.Value.Magnitudes;

        var m = data.GetLength(0);
        var n = data.GetLength(1);

        var dx = (times.Last() - times.First()) / m;
        var left = times.First() - (dx * 0.5);
        var right = times.Last() + (dx * 0.5);

        var dy = (freqs.Last() - freqs.First()) / n;
        var bottom = freqs.First() - (dy * 0.5);
        var top = freqs.Last() + (dy * 0.5);

        var v00 = new DataPoint(left, bottom);
        var v11 = new DataPoint(right, top);

        var s00 = Transform(v00);
        var s11 = Transform(v11);

        var rect = new OxyRect(s00, s11);

        rc.DrawImage(
            Image,
            rect.Left, rect.Top, rect.Width, rect.Height,
            1, false);
    }

    protected override void UpdateMaxMin()
    {
        base.UpdateMaxMin();

        if (Spectrogram == null)
        {
            return;
        }

        var times = Spectrogram.Value.Timestamps;
        var freqs = Spectrogram.Value.Frequencies;
        var magns = Spectrogram.Value.Magnitudes;

        var m = magns.GetLength(0);
        var n = magns.GetLength(1);

        MinX = times.First();
        MaxX = times.Last();

        MinY = freqs.First();
        MaxY = freqs.Last();

        if (XAxis.IsLogarithmic())
        {
            var gx = Math.Log(MaxX / MinX) / (m - 1);

            MinX *= Math.Exp(gx / -2);
            MaxX *= Math.Exp(gx / 2);
        }
        else
        {
            var dx = (MaxX - MinX) / (m - 1);

            MinX -= dx / 2;
            MaxX += dx / 2;
        }

        if (YAxis.IsLogarithmic())
        {
            var gy = Math.Log(MaxY / MinY) / (n - 1);

            MinY *= Math.Exp(gy / -2);
            MaxY *= Math.Exp(gy / 2);
        }
        else
        {
            var dy = (MaxY - MinY) / (n - 1);

            MinY -= dy / 2;
            MaxY += dy / 2;
        }
    }

    public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
    {
        return base.GetNearestPoint(point, interpolate);
    }
}
