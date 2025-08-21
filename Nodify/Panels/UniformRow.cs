using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// UniformRow is used to arrange children in a single row with all equal cell sizes.
    /// </summary>
    public class UniformRow : Panel
    {
        public static double CellWidth { get; private set; }

        // Dependency property for margin between items
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register(
                "ItemMargin",
                typeof(double),
                typeof(UniformRow),
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double ItemMargin
        {
            get { return (double)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return MeasureOverride(constraint, InternalChildren, ItemMargin);
        }

        public static Size MeasureOverride(Size constraint, IList internalChildren, double itemMargin = 5.0)
        {
            if (internalChildren.Count == 0)
                return new Size(0, 0);

            int visibleChildren = GetVisibleChildrenCount(internalChildren);
            if (visibleChildren == 0)
                return new Size(0, 0);

            // Calculate available width accounting for margins between items
            double totalMarginWidth = (visibleChildren - 1) * itemMargin;
            double availableWidthForChildren = constraint.Width - totalMarginWidth;

            Size childConstraint = new Size(availableWidthForChildren / visibleChildren, constraint.Height);
            double maxChildDesiredWidth = 0.0;
            double maxChildDesiredHeight = 0.0;

            // Measure each child, keeping track of maximum desired width and height.
            for (int i = 0; i < internalChildren.Count; i++)
            {
                UIElement child = internalChildren[i] as UIElement;

                if (child.Visibility == Visibility.Collapsed)
                    continue;

                // Measure the child.
                child.Measure(childConstraint);
                Size childDesiredSize = child.DesiredSize;

                if (maxChildDesiredWidth < childDesiredSize.Width)
                {
                    maxChildDesiredWidth = childDesiredSize.Width;
                }

                if (maxChildDesiredHeight < childDesiredSize.Height)
                {
                    maxChildDesiredHeight = childDesiredSize.Height;
                }
            }

            // Return the total desired size including margins
            double totalDesiredWidth = (maxChildDesiredWidth * visibleChildren) + totalMarginWidth;
            return new Size(totalDesiredWidth, maxChildDesiredHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return ArrangeOverride(InternalChildren, arrangeSize, ItemMargin);
        }

        public static Size ArrangeOverride(IList internalChildren, Size arrangeSize, double itemMargin = 5.0)
        {
            if (internalChildren.Count == 0)
                return arrangeSize;

            int visibleChildren = GetVisibleChildrenCount(internalChildren);
            if (visibleChildren == 0)
                return arrangeSize;

            // Calculate cell width accounting for margins
            double totalMarginWidth = (visibleChildren - 1) * itemMargin;
            double availableWidthForChildren = arrangeSize.Width - totalMarginWidth;
            CellWidth = availableWidthForChildren / visibleChildren;

            double currentX = 0;
            bool isFirstItem = true;

            // Arrange and Position each child to the same cell size
            foreach (UIElement child in internalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    // Still need to arrange collapsed children (required by WPF)
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                // Add margin before each item except the first
                if (!isFirstItem)
                {
                    currentX += itemMargin;
                }
                isFirstItem = false;

                // Arrange child to fill the entire cell
                Rect childBounds = new Rect(currentX, 0, CellWidth, arrangeSize.Height);
                AttachedProperties.SetLocation(child, new Point(currentX, 0));
                child.Arrange(childBounds);

                currentX += CellWidth;
            }

            return arrangeSize;
        }

        private static int GetVisibleChildrenCount(IList internalChildren)
        {
            int count = 0;
            foreach (UIElement child in internalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                    count++;
            }
            return count;
        }
    }

    public class AttachedProperties : UIElement
    {
        // Register an attached dependency property with the specified
        // property name, property type, owner type, and property metadata.
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.RegisterAttached(
          "Location",
          typeof(Point),
          typeof(AttachedProperties),
          new FrameworkPropertyMetadata(defaultValue: default(Point),
              flags: FrameworkPropertyMetadataOptions.AffectsRender)
        );

        // Declare a get accessor method.
        public static Point GetLocation(UIElement target) =>
            (Point)target.GetValue(LocationProperty);

        // Declare a set accessor method.
        public static void SetLocation(UIElement target, Point value) =>
            target.SetValue(LocationProperty, value);
    }
}