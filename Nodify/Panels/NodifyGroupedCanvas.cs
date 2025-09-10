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
    /// <summary>Defines the positioning type for canvas items.</summary>
    public enum ItemPositionType
    {
        Left,
        Right,
        Center
    }



    /// <summary>A canvas like panel that works with <see cref="INodifyCanvasItem"/>s.</summary>
    public class NodifyGroupedCanvas : Panel
    {
        const int RowSpace = 100;
        const int ColumnSpace = 400;
        const int GroupRowSpacing = 150; // Extra spacing between groups
        const int TypeColumnSpacing = 50; // Spacing between left/right type columns

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached(
          "Index",
          typeof(int),
          typeof(NodifyGroupedCanvas),
          new FrameworkPropertyMetadata(defaultValue: 0, flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.RegisterAttached(
          "ItemType",
          typeof(ItemPositionType),
          typeof(NodifyGroupedCanvas),
          new FrameworkPropertyMetadata(defaultValue: ItemPositionType.Center, flags: FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GroupIdProperty =
            DependencyProperty.RegisterAttached(
          "GroupId",
          typeof(int),
          typeof(NodifyGroupedCanvas),
          new FrameworkPropertyMetadata(defaultValue: 0));

        //public static int GetIndex(UIElement target) => (int)target.GetValue(IndexProperty);
        //public static void SetIndex(UIElement target, int value) => target.SetValue(IndexProperty, value);

        //public static ItemPositionType GetItemType(UIElement target) => (ItemPositionType)target.GetValue(ItemTypeProperty);
        //public static void SetItemType(UIElement target, ItemPositionType value) => target.SetValue(ItemTypeProperty, value);

        //public static int GetGroupId(UIElement target) => (int)target.GetValue(GroupIdProperty);
        //public static void SetGroupId(UIElement target, int value) => target.SetValue(GroupIdProperty, value);



        public static readonly DependencyProperty ExtentProperty = DependencyProperty.Register(nameof(Extent), typeof(Rect), typeof(NodifyGroupedCanvas), new FrameworkPropertyMetadata(BoxValue.Rect));

        /// <summary>The area covered by the children of this panel.</summary>
        public Rect Extent
        {
            get => (Rect)GetValue(ExtentProperty);
            set => SetValue(ExtentProperty, value);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var extent = ArrangeOverride(InternalChildren, new Rect(new Point(0, 0), arrangeSize));
            Extent = extent;
            return new Size(extent.Width, extent.Height);
        }

        private class GroupedItem
        {
            public INodifyCanvasItem Item { get; set; }
            public UIElement Element { get; set; }
            public int Index { get; set; }
            public ItemPositionType Type { get; set; }
            public string GroupId { get; set; }
        }

        public static Rect ArrangeOverride(IList children, Rect rect)
        {
            if (children.Count == 0)
                return new Rect(0, 0, 0, 0);

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            // Group items by GroupId and ItemType
            var groupedItems = new List<GroupedItem>();
            for (int i = 0; i < children.Count; i++)
            {
                var element = (UIElement)children[i];
                var item = (INodifyCanvasItem)children[i];

                groupedItems.Add(new GroupedItem
                {
                    Item = item,
                    Element = element,
                    Type = GetItemType(element),
                    GroupId = GetGroupKey(element)
                });
            }

            // Group by GroupId first, then by Type within each group
            var groups = groupedItems
                .GroupBy(x => x.GroupId)
                .OrderBy(g => g.Key)
                .ToList();

            double currentRowY = rect.Y;

            foreach (var group in groups)
            {
                // Separate items by type within this group
                var leftItems = group.Where(x => x.Type == ItemPositionType.Left).OrderBy(x => x.Index).ToList();
                var rightItems = group.Where(x => x.Type == ItemPositionType.Right).OrderBy(x => x.Index).ToList();
                var centerItems = group.Where(x => x.Type == ItemPositionType.Center).OrderBy(x => x.Index).ToList();

                // Calculate layout for this group
                var groupStartY = currentRowY;
                var groupMaxY = currentRowY;

                // Position left items
                if (leftItems.Any())
                {
                    var leftMaxY = PositionItemsInColumn(leftItems, rect.X, currentRowY, ColumnSpace, RowSpace, ref minX, ref minY, ref maxX, ref maxY);
                    groupMaxY = Math.Max(groupMaxY, leftMaxY);
                }

                // Position right items
                if (rightItems.Any())
                {
                    // Calculate right column starting position
                    var rightColumnX = rect.X + rect.Width - ColumnSpace;
                    if (rightColumnX <= rect.X + ColumnSpace + TypeColumnSpacing)
                    {
                        // If there's not enough space, position to the right of left items
                        rightColumnX = rect.X + ColumnSpace + TypeColumnSpacing;
                    }

                    var rightMaxY = PositionItemsInColumn(rightItems, rightColumnX, currentRowY, ColumnSpace, RowSpace, ref minX, ref minY, ref maxX, ref maxY);
                    groupMaxY = Math.Max(groupMaxY, rightMaxY);
                }

                // Position center items (between left and right, or in center if no left/right items)
                if (centerItems.Any())
                {
                    var centerColumnX = rect.X + rect.Width / 2 - ColumnSpace / 2;

                    // If we have left items, position center items to the right of them
                    if (leftItems.Any())
                    {
                        centerColumnX = rect.X + ColumnSpace + TypeColumnSpacing;
                    }

                    // If we have right items and left items, position center items between them
                    if (leftItems.Any() && rightItems.Any())
                    {
                        var leftEnd = rect.X + ColumnSpace + TypeColumnSpacing;
                        var rightStart = rect.X + rect.Width - ColumnSpace - TypeColumnSpacing;
                        centerColumnX = leftEnd + (rightStart - leftEnd) / 2 - ColumnSpace / 2;
                    }

                    var centerMaxY = PositionItemsInColumn(centerItems, centerColumnX, currentRowY, ColumnSpace, RowSpace, ref minX, ref minY, ref maxX, ref maxY);
                    groupMaxY = Math.Max(groupMaxY, centerMaxY);
                }

                // Update current row position for next group
                currentRowY = groupMaxY + GroupRowSpacing;
            }

            var extent = minX == double.MaxValue
                ? new Rect(0, 0, 0, 0)
                : new Rect(minX, minY, maxX - minX, maxY - minY);

            return extent;
        }

        private static double PositionItemsInColumn(List<GroupedItem> items, double columnX, double startY, double columnSpacing, double rowSpacing, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            var currentY = startY;
            var columnCount = 1; // Start with single column, can be expanded if needed

            // For large numbers of items in one type, create sub-columns
            if (items.Count > 8)
            {
                columnCount = (int)Math.Ceiling(Math.Sqrt(items.Count / 2.0));
            }

            for (int i = 0; i < items.Count; i++)
            {
                var groupedItem = items[i];
                var subColumn = i % columnCount;
                var row = i / columnCount;

                var point = new Point(
                    columnX + subColumn * (columnSpacing / 2),
                    startY + row * rowSpacing + (subColumn % 2 * 25) // Slight offset for visual appeal
                );

                groupedItem.Item.Location = point;
                groupedItem.Item.Arrange(new Rect(groupedItem.Item.Location, groupedItem.Item.DesiredSize));

                ChangeExtent(ref minX, ref minY, ref maxX, ref maxY, groupedItem.Element, groupedItem.Item.Location);

                currentY = Math.Max(currentY, point.Y + groupedItem.Item.DesiredSize.Height);
            }

            return currentY;
        }

        private static void ChangeExtent(ref double minX, ref double minY, ref double maxX, ref double maxY, UIElement child, Point location)
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

        public static string GetGroupKey(UIElement element, string? KeyPropertyName = "GroupKey")
        {
            //if (element is ITreeIndex directItem)
            //    return directItem;
            //else if (element is FrameworkElement fe && fe.DataContext is ITreeIndex contextItem)
            //    return contextItem;
            //else if (element.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } property)
            //{
            //    var key = property.GetValue(element) as string;
            //    if (key != null)
            //    {
            //        if (Utility.Structs.Index.ParseKeyToPath(key) is { } arr && arr.Length > 0)
            //            return new TreeNode((Utility.Structs.Index)key) { Element = element };
            //    }
            //}
            if (element is FrameworkElement _fe && _fe.DataContext.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } _property)
            {
                var key = _property.GetValue(_fe.DataContext) as string;
                if (key != null)
                {
                    return key.ToString();
                }
            }
            return null;
            //throw new Exception($"Element of type {element.GetType()} does not implement ITreeIndex or have a valid DataContext with ITreeIndex or KeyPropertyName '{KeyPropertyName}'.");
        }

        public static ItemPositionType GetItemType(UIElement element)
        {
            //if (element is ITreeIndex directItem)
            //    return directItem;
            //else if (element is FrameworkElement fe && fe.DataContext is ITreeIndex contextItem)
            //    return contextItem;
            //else if (element.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } property)
            //{
            //    var key = property.GetValue(element) as string;
            //    if (key != null)
            //    {
            //        if (Utility.Structs.Index.ParseKeyToPath(key) is { } arr && arr.Length > 0)
            //            return new TreeNode((Utility.Structs.Index)key) { Element = element };
            //    }
            //}

            if (element is FrameworkElement { DataContext: { } dataContext } _fe)
            {
                var data = dataContext.GetType().GetProperty("Data")?.GetValue(dataContext);
                var str = data?.GetType().ToString();
                if (str?.Contains("Observable") == true)
                    return ItemPositionType.Left;
                if (str?.Contains("Ref5") == true)
                    return ItemPositionType.Left;
            }
            return ItemPositionType.Center;
            //throw new Exception($"Element of type {element.GetType()} does not implement ITreeIndex or have a valid DataContext with ITreeIndex or KeyPropertyName '{KeyPropertyName}'.");
        }

    }
}