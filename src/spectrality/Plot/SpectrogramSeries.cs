using System;
using OxyPlot;
using OxyPlot.Series;
using Spectrality.Models;

namespace Spectrality.Plot;

public class SpectrogramSeries : XYAxisSeries
{
  private SpectrogramImage SpectrogramImage { get; set; } = new((-120, 0));
  private ICoordinateTransformationModel<DataPoint> CoordinateTransformation { get; set; }

  public Spectrogram? Spectrogram { get; private set; }
  public OxyImage? Image { get; private set; }

  public SpectrogramSeries(Spectrogram spectrogram)
  {
    Spectrogram = spectrogram;
    Image = SpectrogramImage.GetImage(spectrogram);

    CoordinateTransformation = new CartesianCoordinateTransformationModel(
      new LinearCoordinateTransformationModel(spectrogram.Timestamps),
      new LogarithmicCoordinateTransformationModel(spectrogram.Frequencies));
  }

  public override void Render(IRenderContext rc)
  {
    if (Spectrogram == null)
    {
      return;
    }

    var spectrogram = Spectrogram.Value;
    var magns = spectrogram.Magnitudes;
    var image = Image ??= SpectrogramImage.GetImage(spectrogram);

    double left = 0;
    double right = magns.GetLength(0);
    double bottom = magns.GetLength(1);
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
    var magns = spectrogram.Magnitudes;

    double left = 0;
    double right = magns.GetLength(0);
    double bottom = magns.GetLength(1);
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
    var magns = spectrogram.Magnitudes;

    var virtualPoint = InverseTransform(screenPoint);
    var dataPoint = CoordinateTransformation.Backward(virtualPoint);

    double left = 0;
    double right = magns.GetLength(0);
    double bottom = magns.GetLength(1);
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

    var valueX = Math.Round(nearestVirtualPoint.X, 3);
    var valueY = Math.Round(nearestVirtualPoint.Y, 1);
    var valueZ = Math.Round(magns[nearestX, nearestY], 1);

    var text = string.Join(Environment.NewLine,
    [
      $"Timestamp: {valueX} s",
      $"Frequency: {valueY} Hz",
      $"Magnitude: {valueZ} dB"
    ]);

    return new TrackerHitResult
    {
      Series = this,
      DataPoint = nearestVirtualPoint,
      Position = trackerScreenPoint,
      Text = text
    };
  }
}
