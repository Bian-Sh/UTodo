  
using System.Collections.Generic;
using UnityEditor; 
using UnityEditor.IMGUI.Controls;
using UnityEngine; 

namespace UTODO
{
    public class TodolistTree : TreeView
    {  
        public TodolistTree(TreeViewState state) : base(state)
        {
            Reload();
        }
        public TodolistTree(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        private TreeViewItem root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        private TreeViewItem red = new TreeViewItem { id = 1, depth = 0, displayName = "Level Red" };
        private TreeViewItem yellow = new TreeViewItem { id = 2, depth = 0, displayName = "Level Yellow" };
        private TreeViewItem green = new TreeViewItem { id = 3, depth = 0, displayName = "Level Green" };

        protected override TreeViewItem BuildRoot()
        {
            root.AddChild(red);
            root.AddChild(yellow);
            root.AddChild(green);
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        public void AddChild( TreeViewItem child) 
        {
            red.AddChild(child);
        }

        public void Reset()
        {
            if (red.children != null)
                red.children.Clear();
            if (yellow.children != null)
                yellow.children.Clear();
            if (green.children != null)
                green.children.Clear();
        }
         
        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
        } 

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            // 设置
        }

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
            // 
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
        }
    } 
}