﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.Features.Home
{
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel();
        }

        internal override IEnumerable<VisualElement> GetStateAwareVisualElements() => new VisualElement[]
        {
            refreshButton,
            stateAwareStackLayout,
        };
    }
}