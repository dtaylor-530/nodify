﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using Utility.WPF.Panels;

namespace Nodify
{
    public class CompositePanel : Panel
    {

        public static readonly DependencyProperty ExtentProperty = DependencyProperty.Register(nameof(Extent), typeof(Rect), typeof(CompositePanel), new FrameworkPropertyMetadata(BoxValue.Rect));

        public CompositePanel()
        {
        }

        public Rect Extent
        {
            get => (Rect)GetValue(ExtentProperty);
            set => SetValue(ExtentProperty, value);
        }

        List<UIElement> listTree = new List<UIElement>();
        List<UIElement> listStandard = new List<UIElement>();

        protected override Size MeasureOverride(Size availableSize)
        {
            var halfWidth = new Size(availableSize.Width / 2, availableSize.Height);


            listTree.Clear();
            listStandard.Clear();
            for (int i = 0; i < Children.Count; i++)
            {
                if (TreePanel.GetTreeItem(Children[i], "Key") is { })
                    listTree.Add(Children[i]);
                else
                    listStandard.Add(Children[i]);
            }

            var treeNodes = TreePanel.BuildTreeStructure(listTree, "Key");
            double maxWidth = 0;
            double totalHeight = 0;

            TreePanel.MeasureTreeNodes(treeNodes, availableSize, 20, 20, ref maxWidth, ref totalHeight);


            var measure2 = NodifyGroupedCanvas.MeasureOverride(listStandard, availableSize);

            return new Size(maxWidth + measure2.Width, totalHeight + measure2.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            double maxX = 0;
            double maxY = double.MinValue;

            var halfWidth = finalSize.Width / 2;

            //if (Arrangement == Arrangement.Tree)
            //{
            var treeNodes = TreePanel.BuildTreeStructure(listTree, "Key");
            var rootNodes = TreePanel.rootNodes(treeNodes);
            double currentY = 0;
            TreePanel.ArrangeTreeNodes(rootNodes, 0, 100, 100, ref currentY);
            foreach (var node in treeNodes)
            {
                changeExtent(ref minX, ref minY, ref maxX, ref maxY, new Rect(((ILocation)node.Element).Location, node.Element.RenderSize));
            }

            var x = NodifyGroupedCanvas.ArrangeOverride(listStandard, new Rect(new Point(maxX +100, 0), new Size(maxX + 100, 0)));


            for (int i = 0; i < listStandard.Count; i++)
            {
                var child = listStandard[i];
                //changeExtent(ref minX, ref minY, ref maxX, ref maxY, child, item.Location);
                if (child is ILocation location)
                {
                    changeExtent(ref minX, ref minY, ref maxX, ref maxY, new Rect(location.Location, new Size(child.RenderSize.Width, child.RenderSize.Height)));
                }
            }

            Extent = minX == double.MaxValue
               ? new Rect(0, 0, 0, 0)
               : new Rect(minX, minY, maxX - minX, maxY - minY);

            return finalSize;
        }

        private static void changeExtent(ref double minX, ref double minY, ref double maxX, ref double maxY, Rect rect)
        {

            if (rect.X < minX)
            {
                minX = rect.X;
            }

            if (rect.Y < minY)
            {
                minY = rect.Y;
            }

            double sizeX = rect.X + rect.Width;
            if (sizeX > maxX)
            {
                maxX = sizeX;
            }

            double sizeY = rect.Y + rect.Height;
            if (sizeY > maxY)
            {
                maxY = sizeY;
            }
        }

    }
}