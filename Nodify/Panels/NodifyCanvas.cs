using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Panels;
using Size = System.Windows.Size;
using Point = System.Windows.Point;
using System.Linq;

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
        const int space = 300;

        public static readonly DependencyProperty ArrangementProperty =
            DependencyProperty.Register("Arrangement", typeof(Arrangement), typeof(NodifyCanvas), new PropertyMetadata());


        // Register an attached dependency property with the specified
        // property name, property type, owner type, and property metadata.
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

        public Arrangement Arrangement
        {
            get { return (Arrangement)GetValue(ArrangementProperty); }
            set { SetValue(ArrangementProperty, value); }
        }
             
        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            double maxX = double.MinValue;
            double maxY = double.MinValue;

            if (Arrangement == Arrangement.Tree)
            {
                var treeNodes = TreePanel.BuildTreeStructure(Children, "Key");
                var rootNodes = TreePanel.rootNodes(treeNodes);
                double currentY = 0;
                TreePanel.ArrangeTreeNodes(rootNodes, 0, 100, 100, ref currentY);
                foreach (var node in treeNodes)
                {
                    changeExtent(ref minX, ref minY, ref maxX, ref maxY, node.Element, ((ILocation)node.Element).Location);
                }
                Extent = minX == double.MaxValue
                    ? new Rect(0, 0, 0, 0)
                    : new Rect(minX, minY, maxX - minX, maxY - minY);
                return arrangeSize;
            }
            else if (Arrangement == Arrangement.UniformRow)
            {
                var x = UniformRow.ArrangeOverride(Children, arrangeSize, 40);
                Extent = new Rect(0,0, arrangeSize.Width, arrangeSize.Height);
                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    var item = (INodifyCanvasItem)InternalChildren[i];
                    var point = AttachedProperties.GetLocation(InternalChildren[i]);
                    item.Location = point;
                }
                return x;
            }

            HashSet<int> indices = new HashSet<int>();

            UIElementCollection children = InternalChildren;
            var sqrtRoot = (int)System.Math.Round(System.Math.Sqrt(children.Count));
            int columnCount = sqrtRoot;
            for (int i = 0; i < children.Count; i++)
            {
                var item = (INodifyCanvasItem)children[i];


                //if (item.Location != default)
                //{

                //}

                int index = NodifyCanvas.GetIndex(children[i]);
                while (indices.Add(index) == false)
                {
                    index++;
                }
                var row = (int)(index / columnCount);
                var column = index % columnCount;
                int rowSpace = space;
                int columnSpace = space;
                var point = new Point(column * columnSpace, (row * rowSpace) + (column % 2 * 50));
                item.Location = point;
                item.Arrange(new Rect(item.Location, item.DesiredSize));

                changeExtent(ref minX, ref minY, ref maxX, ref maxY, children[i], item.Location);
            }

            Extent = minX == double.MaxValue
                ? new Rect(0, 0, 0, 0)
                : new Rect(minX, minY, maxX - minX, maxY - minY);

            return arrangeSize;
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

            if (Arrangement == Arrangement.Tree)
            {
                var treeNodes = TreePanel.BuildTreeStructure(Children, "Key");
                double maxWidth = 0;
                double totalHeight = 0;

                TreePanel.MeasureTreeNodes(treeNodes, constraint, 20, 20, ref maxWidth, ref totalHeight);

                return new Size(maxWidth + 200, totalHeight + 200);
            }
            else if (Arrangement == Arrangement.UniformRow)
            {
                return UniformRow.MeasureOverride(constraint, Children, 40);
            }


            var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            UIElementCollection children = InternalChildren;

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Measure(availableSize);
            }

            return default;
        }
    }
}
