using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Panels;
using Size = System.Windows.Size;
using Point = System.Windows.Point;
using System.Linq;
using System.Collections;

namespace Nodify
{
    /// <summary>Interface for items inside a <see cref="NodifyCanvas"/>.</summary>
    public interface INodifyCanvasItem: ILocation
    {
        /// <summary>The desired size of the item.</summary>
        Size DesiredSize { get; }

        /// <inheritdoc cref="UIElement.Arrange(Rect)" />
        void Arrange(Rect rect);
    }

    /// <summary>A canvas like panel that works with <see cref="INodifyCanvasItem"/>s.</summary>
    public class NodifyCanvas : Panel
    {
        const int RowSpace = 100;
        const int ColumnSpace = 200;

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached(
          "Index",
          typeof(int),
          typeof(NodifyCanvas),
          new FrameworkPropertyMetadata(defaultValue: 0, flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public static int GetIndex(UIElement target) => (int)target.GetValue(IndexProperty);

        public static void SetIndex(UIElement target, int value) => target.SetValue(IndexProperty, value);


        public static readonly DependencyProperty ExtentProperty = DependencyProperty.Register(nameof(Extent), typeof(Rect), typeof(NodifyCanvas), new FrameworkPropertyMetadata(BoxValue.Rect));

        /// <summary>The area covered by the children of this panel.</summary>
        public Rect Extent
        {
            get => (Rect)GetValue(ExtentProperty);
            set => SetValue(ExtentProperty, value);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var extent = ArrangeOverride(InternalChildren, new Rect(new Point(0,0), arrangeSize));
            Extent = extent;
            return new Size(extent.Width, extent.Height);
        }


        public static Rect ArrangeOverride(IList children, Rect rect)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            double maxX = double.MinValue;
            double maxY = double.MinValue;



            HashSet<int> indices = new HashSet<int>();

            var sqrtRoot = (int)System.Math.Round(System.Math.Sqrt(children.Count));
            int columnCount = sqrtRoot;
            for (int i = 0; i < children.Count; i++)
            {
                var item = (INodifyCanvasItem)children[i];
                int index = NodifyCanvas.GetIndex((UIElement)children[i]);
                while (indices.Add(index) == false)
                {
                    index++;
                }
                var row = (int)(index / columnCount);
                var column = index % columnCount;
                int rowSpace = RowSpace;
                int columnSpace = ColumnSpace;
                var point = new Point(column * columnSpace + rect.X, (row * rowSpace) + (column % 2 * 50) + rect.Y);
                item.Location = point;
                item.Arrange(new Rect(item.Location, item.DesiredSize));

                changeExtent(ref minX, ref minY, ref maxX, ref maxY, (UIElement)children[i], item.Location);
            }

            var extent = minX == double.MaxValue
                ? new Rect(0, 0, 0, 0)
                : new Rect(minX, minY, maxX - minX, maxY - minY);

            return extent;
        }

        private static void changeExtent(ref double minX, ref double minY, ref double maxX, ref double maxY, UIElement child, Point location)
        {
            Size size = child.RenderSize;

            if (location.X < minX)
            {
                minX = location.X;
            }

            if (location.Y < minY)
            {
                minY = location.Y;
            }

            double sizeX = location.X + size.Width;
            if (sizeX > maxX)
            {
                maxX = sizeX;
            }

            double sizeY = location.Y + size.Height;
            if (sizeY > maxY)
            {
                maxY = sizeY;
            }
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            return MeasureOverride(InternalChildren, constraint);
        }

        public static Size MeasureOverride(IList children, Size constraint)
        {
            var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

            for (int i = 0; i < children.Count; i++)
            {
                ((UIElement)children[i]).Measure(availableSize);
            }

            return default;
        }
    }
}
