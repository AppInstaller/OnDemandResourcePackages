using Windows.UI.Xaml.Shapes;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.UI.Input;
using System;

namespace CoffeeUniversal.Helpers
{
	public class XamlInkRenderer
	{

		#region Init

		// We use dictionaries because several pointers may be contributing to the live render at a time.
		// The dictionaries are indexed by the pointer ID. A new entry is added to the dictionaries by EnterLiveRenderingMode,
		// it is updated by UpdateLiveRender, and it is removed from the dictionaries by ExitLiveRendering.
		// In principle we don't need a dictionary for the polylines as we could walk our way down from the
		// corresponding path, however it is convenient to have one.
		readonly Dictionary<uint, Path> livePaths = null;
		readonly Dictionary<uint, PolyLineSegment> liveStrokes = null;
		readonly Dictionary<InkStroke, Path> bezierPaths = null;
		readonly Panel render = null;

		public XamlInkRenderer(Panel panel)
		{
			render = panel;
			livePaths = new Dictionary<uint, Path>();
			liveStrokes = new Dictionary<uint, PolyLineSegment>();
			bezierPaths = new Dictionary<InkStroke, Path>();
		}

		#endregion


		#region Live render

		public void EnterLiveRendering(PointerPoint pointerPoint, InkDrawingAttributes drawingAttributes)
		{
			uint pointerId = pointerPoint.PointerId;

			// Create and initialize the data structures necessary to render a polyline in XAML.
			PolyLineSegment stroke = new PolyLineSegment();
			stroke.Points.Add(pointerPoint.Position);
			PathFigure figure = new PathFigure();
			figure.StartPoint = pointerPoint.Position;
			figure.Segments.Add(stroke);
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			Path path = new Path();
			path.Data = geometry;

			path.Stroke = new SolidColorBrush(drawingAttributes.Color);
			path.StrokeThickness = drawingAttributes.Size.Width;
			path.StrokeLineJoin = PenLineJoin.Round;
			path.StrokeStartLineCap = PenLineCap.Round;

			liveStrokes.Add(pointerId, stroke);
			livePaths.Add(pointerId, path);

			// Add path to render so that it is rendered (on top of all the elements with same ZIndex).
			// We want the live render to be on top of the Bezier render, so we set the ZIndex of the elements of the
			// live render to 2 and that of the elements of the Bezier render to 1.
			render.Children.Add(path);
			Canvas.SetZIndex(path, 2);
		}

		public void UpdateLiveRender(PointerPoint pointerPoint)
		{
			uint pointerId = pointerPoint.PointerId;

			try
			{
				liveStrokes[pointerId].Points.Add(pointerPoint.Position);
			}
			catch (KeyNotFoundException)
			{
				// this pointer is not in live rendering mode - ignore it
			}
		}

		public void ExitLiveRendering(PointerPoint pointerPoint)
		{
			uint pointerId = pointerPoint.PointerId;

			try
			{
				render.Children.Remove(livePaths[pointerId]);
				liveStrokes.Remove(pointerId);
				livePaths.Remove(pointerId);
			}
			catch (KeyNotFoundException)
			{
				// this pointer is not in live rendering mode - ignore it
			}
		}

		public void Clear()
		{
			foreach (Path path in bezierPaths.Values)
			{
				render.Children.Remove(path);
			}
			foreach (Path path in livePaths.Values)
			{
				render.Children.Remove(path);
			}

			bezierPaths.Clear();
			livePaths.Clear();
			liveStrokes.Clear();
		}

		#endregion


		#region AddInk

		public void AddInk(InkStroke stroke)
		{
			try
			{
				Path path = CreateBezierPath(stroke);
				bezierPaths.Add(stroke, path);

				// Add path to render so that it is rendered (on top of all the elements with same ZIndex).
				// We want the live render to be on top of the Bezier render, so we set the ZIndex of the elements of the
				// live render to 2 and that of the elements of the Bezier render to 1.
				render.Children.Add(path);
				Canvas.SetZIndex(path, 1);
			}
			catch (ArgumentException)
			{
				// ink is already present - ignore it
			}
		}

		public static Path CreateBezierPath(InkStroke stroke)
		{
			PathFigure figure = new PathFigure();
			var segments = stroke.GetRenderingSegments().GetEnumerator();
			segments.MoveNext();
			figure.StartPoint = segments.Current.Position;
			while (segments.MoveNext())
			{
				BezierSegment segment = new BezierSegment();
				segment.Point1 = segments.Current.BezierControlPoint1;
				segment.Point2 = segments.Current.BezierControlPoint2;
				segment.Point3 = segments.Current.Position;
				figure.Segments.Add(segment);
			}

			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			Path path = new Path();
			path.Data = geometry;

			path.Stroke = new SolidColorBrush(stroke.DrawingAttributes.Color);
			path.StrokeThickness = stroke.DrawingAttributes.Size.Width;
			path.StrokeLineJoin = PenLineJoin.Round;
			path.StrokeStartLineCap = PenLineCap.Round;

			return path;
		}

		#endregion
	}
}
