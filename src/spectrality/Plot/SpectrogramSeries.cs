using System;
using OxyPlot;
using OxyPlot.Series;
using Spectrality.Models;

namespace Spectrality.Plot;

public class SpectrogramSeries : XYAxisSeries
{
  private ISpectrogramBitmap SpectrogramBitmap { get; init; }
  private ICoordinateTransformation<DataPoint> CoordinateTransformation { get; init; }

  public Spectrogram? Spectrogram { get; private set; }

  public SpectrogramSeries(Spectrogram spectrogram)
  {
    SpectrogramBitmap = new ChromesthesiaSpectrogramBitmap((-120, 0));

    CoordinateTransformation = new CartesianCoordinateTransformation(
      new LinearCoordinateTransformation(spectrogram.Data.X),
      new LogarithmicCoordinateTransformation(spectrogram.Data.Y));

    Spectrogram = spectrogram;
    SpectrogramBitmap.RenderBitmap(spectrogram);
  }

  public override void Render(IRenderContext rc)
  {
    if (Spectrogram == null)
    {
      return;
    }

    var data = Spectrogram.Value.Data;
    var image = Spectrogram.Value.Bitmap.ToOxyImage();

    double left = 0;
    double right = data.Width;
    double bottom = data.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var worldPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var worldPoint11 = CoordinateTransformation.Forward(dataPoint11);

    var screenPoint00 = Transform(worldPoint00);
    var screenPoint11 = Transform(worldPoint11);

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
      MaxX = 0;
      MinY = 0;
      MaxY = 0;
      return;
    }

    var data = Spectrogram.Value.Data;

    double left = 0;
    double right = data.Width;
    double bottom = data.Height;
    double top = 0;

    var dataPoint00 = new DataPoint(left, bottom);
    var dataPoint11 = new DataPoint(right, top);

    var worldPoint00 = CoordinateTransformation.Forward(dataPoint00);
    var worldPoint11 = CoordinateTransformation.Forward(dataPoint11);

    MinX = Math.Min(worldPoint00.X, worldPoint11.X);
    MaxX = Math.Max(worldPoint00.X, worldPoint11.X);

    MinY = Math.Min(worldPoint00.Y, worldPoint11.Y);
    MaxY = Math.Max(worldPoint00.Y, worldPoint11.Y);
  }

  public override TrackerHitResult GetNearestPoint(ScreenPoint screenPoint, bool interpolate)
  {
    if (Spectrogram == null)
    {
      return base.GetNearestPoint(screenPoint, interpolate);
    }

    var data = Spectrogram.Value.Data;
    var meta = Spectrogram.Value.Meta;

    var worldPoint = InverseTransform(screenPoint);
    var dataPoint = CoordinateTransformation.Backward(worldPoint);

    double left = 0;
    double right = data.Width;
    double bottom = data.Height;
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
    var nearestWorldPoint = CoordinateTransformation.Forward(nearestDataPoint);

    var trackerDataPoint = new DataPoint(nearestX + 0.5, nearestY + 0.5);
    var trackerWorldPoint = CoordinateTransformation.Forward(trackerDataPoint);
    var trackerScreenPoint = Transform(trackerWorldPoint);

    var scale = new Scale();

    var table = new string[4, 3]
    {
      { "Note:", scale.GetNote(nearestWorldPoint.Y), "" },
      { meta.Z.Name + ":", data[nearestX, nearestY].ToString("F1"), meta.Z.Unit },
      { meta.Y.Name + ":", nearestWorldPoint.Y.ToString("F1"), meta.Y.Unit },
      { meta.X.Name + ":", nearestWorldPoint.X.ToString("F3"), meta.X.Unit }
    };

    var cols = new int[table.GetLength(1)];

    for (var i = 0; i < table.GetLength(0); i++)
    {
      for (var j = 0; j < table.GetLength(1); j++)
      {
        cols[j] = Math.Max(cols[j], table[i, j].Length);
      }
    }

    var rows = new string[table.GetLength(0)];

    for (var i = 0; i < table.GetLength(0); i++)
    {
      var a = table[i, 0].PadRight(cols[0]);
      var b = table[i, 1].PadLeft(cols[1]);
      var c = table[i, 2].PadRight(cols[2]);

      rows[i] = string.Join(' ', a, b, c);
    }

    var text = string.Join('\n', rows);

    return new TrackerHitResult
    {
      Series = this,
      DataPoint = nearestWorldPoint,
      Position = trackerScreenPoint,
      Text = text
    };
  }
}
