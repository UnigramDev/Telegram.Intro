using Telegram.Intro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Template10.Mvvm;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AngleTestSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TLIntroRenderer _renderer;

        private Visual _layoutVisual;
        private bool _selecting;
        private int _selectedIndex;

        public MainPage()
        {
            InitializeComponent();
            DataContext = new SignInWelcomeViewModel();

            _layoutVisual = ElementCompositionPreview.GetElementVisual(LayoutRoot);

            LayoutRoot.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX | ManipulationModes.TranslateInertia;
            LayoutRoot.ManipulationStarted += LayoutRoot_ManipulationStarted;
            LayoutRoot.ManipulationDelta += LayoutRoot_ManipulationDelta;
            LayoutRoot.ManipulationCompleted += LayoutRoot_ManipulationCompleted;
            LayoutRoot.PointerWheelChanged += LayoutRoot_PointerWheelChanged;

            //Loaded += MainPage_Loaded;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 500));
        }

        private void LayoutRoot_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (_selecting)
            {
                return;
            }

            _selecting = true;

            var point = e.GetCurrentPoint(LayoutRoot);
            var delta = -point.Properties.MouseWheelDelta;
            var width = (float)ActualWidth;

            var current = -(_selectedIndex * width);
            var previous = current + width;
            var next = current - width;

            var maximum = next;
            var minimum = previous;

            if (_selectedIndex == 0)
            {
                minimum = current;
                delta = delta > 0 ? delta : 0;
            }
            else if (_selectedIndex == 5)
            {
                maximum = current;
                delta = delta < 0 ? delta : 0;
            }

            var offset = _layoutVisual.Offset;

            var batch = _layoutVisual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            var animation = _layoutVisual.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0, offset.X);

            if (delta < 0)
            {
                // previous
                _selectedIndex--;
                _renderer.SetPage(_selectedIndex);
                animation.InsertKeyFrame(1, minimum);
            }
            else if (delta > 0)
            {
                // next
                _selectedIndex++;
                _renderer.SetPage(_selectedIndex);
                animation.InsertKeyFrame(1, maximum);
            }
            else
            {
                // back
                animation.InsertKeyFrame(1, current);
            }

            _layoutVisual.StartAnimation("Offset.X", animation);

            batch.Completed += (s, args) =>
            {
                _selecting = false;
            };
            batch.End();

        }

        private void LayoutRoot_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _selecting = true;
        }

        private void LayoutRoot_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                e.Complete();
                return;
            }

            var delta = (float)e.Delta.Translation.X;
            var width = (float)ActualWidth;

            var current = -(_selectedIndex * width);
            var previous = current + width;
            var next = current - width;

            var maximum = next;
            var minimum = previous;

            if (_selectedIndex == 0)
            {
                minimum = 0;
            }
            else if (_selectedIndex == 5)
            {
                maximum = current;
            }

            var offset = _layoutVisual.Offset;
            offset.X = Math.Max(maximum, Math.Min(minimum, offset.X + delta));

            var position = Math.Max(maximum, Math.Min(minimum, offset.X)) / width;
            position += _selectedIndex;

            _layoutVisual.Offset = offset;
            _renderer.SetScroll(-position);
        }

        private void LayoutRoot_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var width = (float)ActualWidth;

            var current = -(_selectedIndex * width);
            var previous = current + width;
            var next = current - width;

            var maximum = next;
            var minimum = previous;

            var offset = _layoutVisual.Offset;
            var position = Math.Max(maximum, Math.Min(minimum, offset.X)) / width;

            position += _selectedIndex;

            Debug.WriteLine(position);

            _renderer.SetScroll(0);

            var batch = _layoutVisual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            var animation = _layoutVisual.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0, offset.X);

            if (position != 0 && (position > 0.3f || e.Velocities.Linear.X > 1.5f))
            {
                // previous
                _selectedIndex--;
                _renderer.SetPage(_selectedIndex);
                animation.InsertKeyFrame(1, minimum);
            }
            else if (position != 0 && (position < -0.3f || e.Velocities.Linear.X < -1.5f))
            {
                // next
                _selectedIndex++;
                _renderer.SetPage(_selectedIndex);
                animation.InsertKeyFrame(1, maximum);
            }
            else
            {
                // back
                animation.InsertKeyFrame(1, current);
            }

            _layoutVisual.StartAnimation("Offset.X", animation);

            batch.Completed += (s, args) =>
            {
                _selecting = false;
            };
            batch.End();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            LayoutRoot.Width = availableSize.Width * 6;
            return size;
        }
























































        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var scroll = FindVisualChild<ScrollViewer>(Flip);
            scroll.ViewChanging += Scroll_ViewChanging;
        }

        private void Scroll_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {

            ////var offset = (float)e.NextView.HorizontalOffset - (int)e.NextView.HorizontalOffset;
            ////if (e.NextView.HorizontalOffset < Flip.SelectedIndex + 2)
            ////{
            ////    _renderer.CurrentScroll = -(1 - offset);
            ////}
            ////else if (e.NextView.HorizontalOffset > Flip.SelectedIndex + 2)
            ////{
            ////    _renderer.CurrentScroll = offset;
            ////}

            _renderer.SetScroll((float)e.NextView.HorizontalOffset - 2 - Flip.SelectedIndex);

            //Debug.WriteLine(e.NextView.HorizontalOffset);

            //if (Flip.SelectedIndex == 1 && e.NextView.HorizontalOffset < 3)
            //{
            //    // From page 1 to 0:
            //    _renderer.CurrentScroll = -(1 - decpart);
            //    Debug.WriteLine("NextView: " + e.NextView.HorizontalOffset + " Decimal part: " + -(1 - decpart));
            //}
            //else if (Flip.SelectedIndex == 1 && e.NextView.HorizontalOffset > 3)
            //{
            //    _renderer.CurrentScroll = decpart;
            //    Debug.WriteLine("NextView: " + e.NextView.HorizontalOffset + " Decimal part: " + decpart);
            //}
        }

        private void swapChainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _renderer = new TLIntroRenderer(swapChainPanel);
            _renderer.Loaded();
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_renderer != null)
            {
                _renderer.SetScroll(0);
                _renderer.SetPage(Flip.SelectedIndex);
            }
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }

    public class SignInWelcomeViewModel : ViewModelBase
    {
        public SignInWelcomeViewModel()
        {
            Items = new ObservableCollection<WelcomeTab>();

            // TODO: put them in a separate file?
            // TODO: localization
            Items.Add(new WelcomeTab { Title = "Unigram", Text = "**Unigram** is a Telegram Universal app built by the Windows Community, for the Windows Community" });
            Items.Add(new WelcomeTab { Title = "Fast", Text = "**Telegram** delivers messages faster\nthan any other application." });
            Items.Add(new WelcomeTab { Title = "Free", Text = "**Telegram** is free forever. No ads.\nNo subscription fees." });
            Items.Add(new WelcomeTab { Title = "Powerful", Text = "**Telegram** has no limits on\nthe size of your media and chats." });
            Items.Add(new WelcomeTab { Title = "Secure", Text = "**Telegram** keeps your messages\nsafe from hacker attacks." });
            Items.Add(new WelcomeTab { Title = "Cloud-Based", Text = "**Telegram** lets you access your\nmessages from multiple devices." });
            SelectedItem = Items[0];
        }

        private WelcomeTab _selectedItem;
        public WelcomeTab SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                //Set(ref _selectedItem, value);
            }
        }

        public ObservableCollection<WelcomeTab> Items { get; private set; }

        public class WelcomeTab
        {
            public string Title { get; set; }

            public string Text { get; set; }
        }
    }

    public static class Markdown
    {
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Markdown), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as TextBlock;
            var markdown = e.NewValue as string;

            sender.Inlines.Clear();

            var previous = 0;
            var index = markdown.IndexOf("**");
            var next = index > -1 ? markdown.IndexOf("**", index + 2) : -1;

            while (index > -1 && next > -1)
            {
                if (index - previous > 0)
                {
                    sender.Inlines.Add(new Run { Text = markdown.Substring(previous, index - previous) });
                }

                sender.Inlines.Add(new Run { Text = markdown.Substring(index + 2, next - index - 2), FontWeight = FontWeights.SemiBold });

                previous = next + 2;
                index = markdown.IndexOf("**", next + 2);
                next = index > -1 ? markdown.IndexOf("**", index + 2) : -1;
            }

            if (markdown.Length - previous > 0)
            {
                sender.Inlines.Add(new Run { Text = markdown.Substring(previous, markdown.Length - previous) });
            }
        }
    }
}
