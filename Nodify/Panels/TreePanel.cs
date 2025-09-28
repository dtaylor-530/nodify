using Nodify.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Size = System.Windows.Size;

namespace Nodify.Panels

{
    public interface IIndex : IReadOnlyList<int>, IComparable<IIndex>, IEquatable<IIndex>, IComparable
    {
        bool IsEmpty { get; }
        int Local { get; }
    }
    public interface ITreeIndex
    {
        IIndex Index { get; }
    }
    public interface ILocation
    {
        Point Location { get; set; }
    }

    public class TreeNode : ITreeIndex, IComparable
    {
        public TreeNode(IIndex index)
        {
            Index = index;
        }
        public IIndex Index { get; }
        public UIElement Element { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
        public int Level => Index?.Count ?? 0;

        public int CompareTo(object? obj)
        {
            if (obj is ITreeIndex { Index: { } index })
                return Index.CompareTo(index);
            throw new Exception("224ik e3s");
        }
    }


    // Custom Panel that arranges items in a hierarchical tree structure
    public class TreePanel : Panel
    {
        public static readonly DependencyProperty IndentSizeProperty = DependencyProperty.Register(nameof(IndentSize), typeof(double), typeof(TreePanel), new PropertyMetadata(20.0, OnLayoutPropertyChanged));
        public static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(TreePanel), new PropertyMetadata(5.0, OnLayoutPropertyChanged));
        public static readonly DependencyProperty KeyPropertyNameProperty = DependencyProperty.Register("KeyPropertyName", typeof(string), typeof(TreePanel), new PropertyMetadata("Key"));


        public IValueConverter IndexConverter
        {
            get { return (IValueConverter)GetValue(IndexConverterProperty); }
            set { SetValue(IndexConverterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexConverterProperty =
            DependencyProperty.Register("IndexConverter", typeof(int), typeof(TreePanel), new PropertyMetadata());


        public string KeyPropertyName
        {
            get { return (string)GetValue(KeyPropertyNameProperty); }
            set { SetValue(KeyPropertyNameProperty, value); }
        }

        public double IndentSize
        {
            get => (double)GetValue(IndentSizeProperty);
            set => SetValue(IndentSizeProperty, value);
        }

        public double ItemSpacing
        {
            get => (double)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreePanel panel)
            {
                panel.InvalidateMeasure();
                panel.InvalidateArrange();
            }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var treeNodes = BuildTreeStructure(Children, KeyPropertyName, IndexConverter);
            double maxWidth = 0;
            double totalHeight = 0;

            MeasureTreeNodes(treeNodes, availableSize, IndentSize, ItemSpacing, ref maxWidth, ref totalHeight);
            return new Size(maxWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var treeNodes = BuildTreeStructure(Children, KeyPropertyName, IndexConverter);
            double currentY = 0;
            ArrangeTreeNodes(treeNodes, 0, IndentSize, ItemSpacing, ref currentY);
            return finalSize;
        }

        public static List<TreeNode> BuildTreeStructure(IEnumerable Children, string? KeyPropertyName = null, IValueConverter? converter= null)
        {
            var allNodes = new List<TreeNode>();

            foreach (UIElement child in Children)
            {
                var index = GetTreeIndex(child, KeyPropertyName, converter);
                if (index != null)
                {
                    var node = new TreeNode(index) 
                    {
                        Element = child,
                    };
                    allNodes.Add(node);
                }
            }

            return allNodes;
        }

        public static List<TreeNode> rootNodes(List<TreeNode> allNodes)
        {
            allNodes.Sort();

            var rootNodes = new List<TreeNode>();
            foreach (var node in allNodes)
            {
                if (node.Level == 1)
                {
                    rootNodes.Add(node);
                }
                else if (node.Level > 1)
                {
                    var parent = FindParentNode(allNodes, node.Index);
                    if (parent == null)
                        throw new Exception("asd34ddd");
                    parent?.Children.Add(node);
                }
            }
            return rootNodes;
        }


        public static IIndex GetTreeIndex(UIElement element, string? KeyPropertyName = null, IValueConverter? indexConverter = null)
        {
            if (element is ITreeIndex directItem)
                return directItem.Index;
            else if (element is FrameworkElement fe && fe.DataContext is ITreeIndex contextItem)
                return contextItem.Index;
            else if (element is FrameworkElement fe2 && indexConverter?.Convert(fe2.DataContext, typeof(IIndex), default, default) is IIndex index)
                return index;
            else if (element.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } property)
            {
                var key = property.GetValue(element) as string;
                if (key != null)
                {
   
                    if (Index.ParseKeyToPath(key) is { } arr && arr.Length > 0)
                        return (Index)key;

                }
            }
            else if (element is FrameworkElement _fe && _fe.DataContext.GetType().GetProperties().FirstOrDefault(p => p.Name == KeyPropertyName) is { } _property)
            {
                var key = _property.GetValue(_fe.DataContext) as string;
                if (key != null)
                {
                    if (Index.ParseKeyToPath(key) is { } arr && arr.Length > 0)
                    {
                        return (Index)key;
                    }
                }
            }
            return null;
            //throw new Exception($"Element of type {element.GetType()} does not implement ITreeIndex or have a valid DataContext with ITreeIndex or KeyPropertyName '{KeyPropertyName}'.");
        }


        public static TreeNode FindParentNode(List<TreeNode> allNodes, IReadOnlyList<int> childPath)
        {
            if (childPath == null || childPath.Count <= 1)
                return null;

            var parentPath = childPath.Take(childPath.Count - 1).ToArray();
            return allNodes.FirstOrDefault(node =>
                node.Index != null &&
                node.Index.Count == parentPath.Length &&
                node.Index.Equals((Index)parentPath));
        }

        public static void MeasureTreeNodes(List<TreeNode> nodes, Size availableSize, double IndentSize, double ItemSpacing, ref double maxWidth, ref double totalHeight)
        {
            foreach (var node in nodes)
            {
                try
                {
                    node.Element.Measure(availableSize);
                    var elementSize = node.Element.DesiredSize;
                    double nodeWidth = (node.Level - 1) * IndentSize + elementSize.Width;
                    maxWidth = Math.Max(maxWidth, nodeWidth);
                    totalHeight += elementSize.Height + ItemSpacing;

                    if (node.Children.Count > 0)
                    {
                        MeasureTreeNodes(node.Children, availableSize, IndentSize, ItemSpacing, ref maxWidth, ref totalHeight);
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }

        public static void ArrangeTreeNodes(List<TreeNode> nodes, int level, double IndentSize, double ItemSpacing, ref double currentY)
        {
            foreach (var node in nodes)
            {
                var elementSize = node.Element.DesiredSize;
                double x = level * IndentSize;
                try
                {
                    node.Element.Arrange(new Rect(x, currentY, Math.Max(50, elementSize.Width), Math.Max(50, elementSize.Height)));

                }
                catch(Exception ex)
                {

                }

                if (node.Element is ILocation location)
                {
                    location.Location = new Point((float)x, (float)currentY);
                }
                currentY += elementSize.Height + ItemSpacing;
                if (node.Children.Count > 0)
                {
                    ArrangeTreeNodes(node.Children, level + 1, IndentSize, ItemSpacing, ref currentY);
                }
            }
        }
    }
}