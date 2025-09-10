using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Panels;

namespace Nodify
{
    /// <summary>Interface for items inside a <see cref="NodifyCanvas"/>.</summary>
    public interface INodifyCanvasItem : ILocation
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
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            double maxX = double.MinValue;
            double maxY = double.MinValue;

            UIElementCollection children = InternalChildren;
            for (int i = 0; i < children.Count; i++)
            {
                var item = (INodifyCanvasItem)children[i];
                item.Arrange(new Rect(item.Location, item.DesiredSize));

                Size size = children[i].RenderSize;

                if (item.Location.X < minX)
                {
                    minX = item.Location.X;
                }

                if (item.Location.Y < minY)
                {
                    minY = item.Location.Y;
                }

                double sizeX = item.Location.X + size.Width;
                if (sizeX > maxX)
                {
                    maxX = sizeX;
                }

                double sizeY = item.Location.Y + size.Height;
                if (sizeY > maxY)
                {
                    maxY = sizeY;
                }
            }

            Extent = minX == double.MaxValue
                ? new Rect(0, 0, 0, 0)
                : new Rect(minX, minY, maxX - minX, maxY - minY);

            return arrangeSize;
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
