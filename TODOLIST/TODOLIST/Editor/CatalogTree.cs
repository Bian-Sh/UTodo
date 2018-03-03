 
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls; 

namespace UTODO
{
    public class CatalogTree : TreeView
    {
        public Action<int> contextClick = null;
        public Action refreshList = null;
        public Action<int> contextDoubleClick = null;

        public CatalogTree(TreeViewState state) : base(state)
        {
            Reload();
        } 
        public CatalogTree(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        private TreeViewItem root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        private TreeViewItem all = new TreeViewItem { id = 1, depth = 0, displayName = "All" };
        private TreeViewItem csharp = new TreeViewItem { id = 2, depth = 1, displayName = "CSharp" };
        private TreeViewItem lua = new TreeViewItem { id = 3, depth = 1, displayName = "Lua" };

        protected override TreeViewItem BuildRoot()
        { 
            root.AddChild(all);
            all.AddChild(csharp);
            all.AddChild(lua);
            SetupDepthsFromParentsAndChildren(root); 
            return root;
        }

        public void AddSharpChild( TreeViewItem item )
        {
            csharp.AddChild(item);
        }

        public void AddLuaChild(TreeViewItem item)
        {
            lua.AddChild(item);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (null != refreshList)
                refreshList();
            foreach (int id in selectedIds)
                ContextClickedItem(id);
        } 

        protected override void ContextClickedItem(int id)
        { 
            base.ContextClickedItem(id);
            if (null != contextClick)
                contextClick(id);
        }

        protected override void DoubleClickedItem(int id)
        { 
            base.DoubleClickedItem(id);
            if (null != contextDoubleClick)
                contextDoubleClick(id);
        }
    }
} 