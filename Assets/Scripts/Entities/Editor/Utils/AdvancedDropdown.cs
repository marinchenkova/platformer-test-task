using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Entities.Editor.Utils {

    public sealed class AdvancedDropdown<T> : AdvancedDropdown {

        private readonly string _title;
        private readonly Action<T> _onItemSelected;
        private readonly Func<T, string> _getItemName;
        private readonly IEnumerable<T> _items;

        private sealed class Item : AdvancedDropdownItem {

            public readonly T data;

            public Item(T data, string name) : base(name) {
                this.data = data;
            }
        }

        public AdvancedDropdown(string title, IEnumerable<T> items, Func<T, string> getItemName, Action<T> onItemSelected)
            : base(new AdvancedDropdownState())
        {
            _title = title;
            _items = items;
            _onItemSelected = onItemSelected;
            _getItemName = getItemName;

            float width = Mathf.Max(minimumSize.x, 240f);
            float height = 14 * EditorGUIUtility.singleLineHeight;

            minimumSize = new Vector2(width, height);
        }

        protected override AdvancedDropdownItem BuildRoot() {
            var root = new AdvancedDropdownItem(_title);

            foreach (var item in _items) {
                root.AddChild(new Item(item, _getItemName.Invoke(item)));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            base.ItemSelected(item);
            if (item is Item t) _onItemSelected.Invoke(t.data);
        }
    }

}
