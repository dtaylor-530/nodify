
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Nodify.Panels
{
    public class HexagonGrid : Grid
    {
        public static readonly DependencyProperty IsArrangeAutomaticProperty =
      DependencyProperty.Register(nameof(IsArrangeAutomatic), typeof(bool), typeof(HexagonGrid), new PropertyMetadata(false));

        public static readonly DependencyProperty RowsProperty =
    DependencyProperty.Register("Rows", typeof(int), typeof(HexagonGrid),
        new FrameworkPropertyMetadata(-1,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(HexagonGrid),
                new FrameworkPropertyMetadata(-1,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public static readonly DependencyProperty ItemLengthProperty =
    DependencyProperty.Register("ItemLength", typeof(double), typeof(HexagonGrid),
        new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty SpaceLengthProperty =
    DependencyProperty.Register("SpaceLength", typeof(double), typeof(HexagonGrid),
        new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));


        /**
         * If the length of 1 side of the hexagon = S, then:
         * Width = 2 x S
         * Height = S x SQRT(3)
         * Column C starts at C x (0.75 x Width)
         * Row R starts at R x Height
         * A row's uneven columns have an vertical offset of 0.5 x Height
         **/
        #region properties
        /// <summary>
        /// represents the length of 1 side of the hexagon.
        /// </summary>
        public double ItemLength
        {
            get { return (double)GetValue(ItemLengthProperty); }
            set { SetValue(ItemLengthProperty, value); }
        }
        public double SpaceLength
        {
            get { return (double)GetValue(SpaceLengthProperty); }
            set { SetValue(SpaceLengthProperty, value); }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public bool IsArrangeAutomatic
        {
            get { return (bool)GetValue(IsArrangeAutomaticProperty); }
            set { SetValue(IsArrangeAutomaticProperty, value); }
        }
        #endregion properties

        protected override Size MeasureOverride(Size constraint)
        {
            return MeasureOverride((IReadOnlyCollection<UIElement>)InternalChildren, SpaceLength, Rows, Columns);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return ArrangeOverride((IReadOnlyCollection<UIElement>)InternalChildren, arrangeSize, SpaceLength, ItemLength, new Size(), IsArrangeAutomatic, Rows, Columns);
        }

        public static Size MeasureOverride(IReadOnlyCollection<UIElement> listStandard, double panelSize, int rows = -1, int columns = -1)
        {
            double side = panelSize;
            double width = 2 * side;
            double height = side * Math.Sqrt(3.0);
            double colWidth = 0.75 * width;
            double rowHeight = height;

            Size availableChildSize = new Size(width, height);

            foreach (FrameworkElement child in listStandard)
            {
                child.Measure(availableChildSize);
            }

            int actualRows = rows;
            int actualCols = columns;

            // Auto-calculate if both are -1
            if (actualCols == -1)
            {
                actualCols = (int)Math.Ceiling(Math.Sqrt(listStandard.Count));
            }
            if (actualRows == -1)
            {
                actualRows = (int)Math.Ceiling((double)listStandard.Count / actualCols);
            }


            double totalHeight = actualRows * rowHeight;
            if (actualCols > 1)
                totalHeight += 0.5 * rowHeight;
            double totalWidth = actualCols + 0.5 * side;

            Size totalSize = new Size(totalWidth, totalHeight);

            return totalSize;
        }

        public static Size ArrangeOverride(IReadOnlyCollection<UIElement> listStandard, Size arrangeSize, double panelSize, double itemLength = -1, Size? offset = null, bool isArrangeAutomatic = false, int rows = -1, int columns = -1)
        {
            double side = panelSize;
            double width = 2 * side;
            double height = side * Math.Sqrt(3.0);
            double colWidth = 0.75 * width;
            double rowHeight = height;

            Size _childSize = childSize();

            int actualRows = rows;
            int actualCols = columns;

            // Auto-calculate if both are -1
            if (actualCols == -1)
            {
                actualCols = (int)Math.Ceiling(Math.Sqrt(listStandard.Count));
            }

            var enumerator = listStandard.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {

                int row, col;
                var child = enumerator.Current;
                if (isArrangeAutomatic)
                {
                    index++;
                    row = index / actualCols;
                    col = index % actualCols;
                    SetRow(child, row);
                    SetColumn(child, col);
                }
                else
                {
                    row = GetRow(child);
                    col = GetColumn(child);
                }
                double left = col * colWidth;
                double top = row * rowHeight;
                bool isUnevenCol = col % 2 != 0;
                if (isUnevenCol)
                    top += 0.5 * rowHeight;
                var point = new Point(left + offset?.Width ?? 0, top + offset?.Height ?? 0);
                //child.Arrange(new Rect(, _childSize));
                if (enumerator.Current is INodifyCanvasItem item)
                {
                    item.Location = point;
                    item.Arrange(new Rect(point, item.DesiredSize));
                }
            }

            return arrangeSize;

            Size childSize()
            {
                double side = itemLength;
                double width = 2 * side;
                double height = side * Math.Sqrt(3.0);
                return new Size(width, height);
            }
        }

     
    }
}