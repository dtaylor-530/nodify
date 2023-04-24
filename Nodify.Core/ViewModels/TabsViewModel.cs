﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Nodify.Core
{
    public class TabsViewModel : ObservableObject
    {
        private TabViewModel? _selectedEditor;
        private bool _autoSelectNewEditor = true;
        private NodifyObservableCollection<TabViewModel> tabs = new();


        public TabsViewModel()
        {
            AddEditorCommand = new DelegateCommand<object>((a) =>
            {
                AddTab(a ?? Content);
            }
            );
            CloseEditorCommand = new DelegateCommand<Guid>(
                id => Tabs.RemoveOne(editor => editor.Id == id),
                _ => Tabs.Count > 0 && SelectedEditor != null);
            Tabs.WhenAdded((editor) =>
            {
                if (AutoSelectNewEditor || Tabs.Count == 1)
                {
                    SelectedEditor = editor;
                }
                editor.OnOpenInnerCalculator += OnOpenInnerCalculator;
            })
            .WhenRemoved((editor) =>
            {
                editor.OnOpenInnerCalculator -= OnOpenInnerCalculator;
                var childEditors = Tabs.Where(ed => ed.Parent == editor).ToList();
                childEditors.ForEach(ed => Tabs.Remove(ed));
            });
        }

        void AddTab(object content)
        {

            var tab = new TabViewModel
            {
                Name = $"Editor {Tabs.Count + 1}",
                Content = content,
            };
            tab.Close += Tab_Close;
            Tabs.Add(tab);
        }

        protected virtual object Content { get => new EditorViewModel(Diagram.Empty); }

        private void Tab_Close(TabViewModel obj)
        {
            Tabs.RemoveOne(editor => editor.Id == obj.Id);
        }

        public NodifyObservableCollection<TabViewModel> Tabs => tabs;

        public ICommand AddEditorCommand { get; }

        public ICommand CloseEditorCommand { get; }

        public TabViewModel? SelectedEditor
        {
            get => _selectedEditor;
            set => SetProperty(ref _selectedEditor, value);
        }

        public bool AutoSelectNewEditor
        {
            get => _autoSelectNewEditor;
            set => SetProperty(ref _autoSelectNewEditor, value);
        }

        private void OnOpenInnerCalculator(TabViewModel parentEditor, object calculator)
        {
            var editor = Tabs.FirstOrDefault(e => e.Content == calculator);
            if (editor != null)
            {
                SelectedEditor = editor;
            }
            else
            {
                var childEditor = new TabViewModel
                {
                    Parent = parentEditor,
                    Content = calculator,
                    Name = $"[Inner] Editor {Tabs.Count + 1}"
                };
                Tabs.Add(childEditor);
            }
        }

    }
}
