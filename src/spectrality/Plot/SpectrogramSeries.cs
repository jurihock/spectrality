using System;
using OxyPlot;
using OxyPlot.Series;
using Spectrality.Models;

namespace Spectrality.Plot;

public class SpectrogramSeries : XYAxisSeries
{
  private readonly ISpectrogramImage SpectrogramImage;
  private readonly ICoordinateTransformation<DataPoint> CoordinateTransformation;

  public Datagram? Spectrogram { get; private set; }
  public OxyImage? Image { get; private set; }

  public SpectrogramSeries(Datagram spectrogram)
  {
    SpectrogramImage = new ChromesthesiaSpectrogramImage((-120, 0));

    CoordinateTransformation = new CartesianCoordinateTransformation(
      new LinearCoordinateTransformation(spectrogram.X),
      new LogarithmicCoordinateTransformation(spectrogram.Y));

    Spectrogram = spectrogram;
    Image = SpectrogramImage.GetImage(spectrogram);
  }

  public override void Render(IRenderContext rc)
  {
    if (Spectrogram == null)
    {
      return;
    }

    var spectrogram = Spectrogram.Value;
    var image = Image ??= SpectrogramImage.GetImage(spectrogram);

    double left = 0;
    double right = spectrogram.Width;
    double bottom = spectrogram.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var virtualPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var virtualPoint11 = CoordinateTransformation.Forward(dataPoint11);

    var screenPoint00 = Transform(virtualPoint00);
    var screenPoint11 = Transform(virtualPoint11);

    var screenRect = new OxyRect(screenPoint00, screenPoint11);

    left = screenRect.Left;
    top = screenRect.Top;
    right = screenRect.Width;
    bottom = screenRect.Height;

    rc.DrawImage(
      image,
      left, top, right, bottom,
      1, false);
  }

  protected override void UpdateMaxMin()
  {
    base.UpdateMaxMin();

    if (Spectrogram == null)
    {
      MinX = 0;
      MinY = 0;

      MaxX = 0;
      MaxY = 0;

      return;
    }

    var spectrogram = Spectrogram.Value;

    double left = 0;
    double right = spectrogram.Width;
    double bottom = spectrogram.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var virtualPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var virtualPoint11 = CoordinateTransformation.Forward(dataPoint11);

    MinX = Math.Min(virtualPoint00.X, virtualPoint11.X);
    MaxX = Math.Max(virtualPoint00.X, virtualPoint11.X);

    MinY = Math.Min(virtualPoint00.Y, virtualPoint11.Y);
    MaxY = Math.Max(virtualPoint00.Y, virtualPoint11.Y);
  }

  public override TrackerHitResult GetNearestPoint(ScreenPoint screenPoint, bool interpolate)
  {
    if (Spectrogram == null)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    var spectrogram = Spectrogram.Value;

    var virtualPoint = InverseTransform(screenPoint);
    var dataPoint = CoordinateTransformation.Backward(virtualPoint);

    double left = 0;
    double right = spectrogram.Width;
    double bottom = spectrogram.Height;
    double top = 0;

    int nearestX = (int)Math.Floor(dataPoint.X);
    int nearestY = (int)Math.Floor(dataPoint.Y);

    if (nearestX < left || nearestX >= right)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    if (nearestY < top || nearestY >= bottom)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    var nearestDataPoint = new DataPoint(nearestX, nearestY);
    var nearestVirtualPoint = CoordinateTransformation.Forward(nearestDataPoint);

    var trackerDataPoint = new DataPoint(nearestX + 0.5, nearestY + 0.5);
    var trackerVirtualPoint = CoordinateTransformation.Forward(trackerDataPoint);
    var trackerScreenPoint = Transform(trackerVirtualPoint);

    var scale = new Scale();

    var values = new string[4, 3]
    {
      { "Note:", scale.GetNote(nearestVirtualPoint.Y), "" },
      { "Magnitude:", Math.Round(spectrogram[nearestX, nearestY], 1).ToString("F1"), "dB" },
      { "Frequency:", Math.Round(nearestVirtualPoint.Y, 1).ToString("F1"), "Hz" },
      { "Timepoint:", Math.Round(nearestVirtualPoint.X, 3).ToString("F3"), "s" }
    };

    var cols = new int[values.GetLength(1)];

    for (var i = 0; i < values.GetLength(0); i++)
    {
      for (var j = 0; j < values.GetLength(1); j++)
      {
        cols[j] = Math.Max(cols[j], values[i, j].Length);
      }
    }

    var rows = new string[values.GetLength(0)];

    for (var i = 0; i < values.GetLength(0); i++)
    {
      var a = values[i, 0].PadRight(cols[0]);
      var b = values[i, 1].PadLeft(cols[1]);
      var c = values[i, 2].PadRight(cols[2]);

      rows[i] = string.Join(' ', a, b, c);
    }

    var trackerText = string.Join(
      Environment.NewLine, rows);

    return new TrackerHitResult
    {
      Series = this,
      DataPoint = nearestVirtualPoint,
      Position = trackerScreenPoint,
      Text = trackerText
    };
  }
}
