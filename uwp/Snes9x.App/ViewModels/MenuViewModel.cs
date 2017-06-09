//using Snes9x.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Snes9x.ViewModels
//{
//    public class MenuViewModel : BindableBase
//    {
//        private readonly MainViewModel ViewModel;

//        public MenuViewModel(MainViewModel viewModel)
//        {
//            ViewModel = viewModel;
//        }

//        public bool IsStretched
//        {
//            get => _isStretched;
//            set
//            {
//                if (SetValue(ref _isStretched, value))
//                {
//                    ViewModel.Renderer.Stretch = _isStretched;
//                    OnPropertyChanged(nameof(IsAspectRatioEnabled));
//                    OnPropertyChanged(nameof(AreAspectItemsEnabled));
//                }
//            }
//        }

//        public bool IsAspectPreserved
//        {
//            get => _isAspectPreserved;
//            set
//            {
//                if (SetValue(ref _isAspectPreserved, value))
//                {
//                    ViewModel.Renderer.PreserverAspectRatio = _isAspectPreserved;
//                    OnPropertyChanged(nameof(AreAspectItemsEnabled));
//                }
//            }
//        }

//        public bool IsAspect4x3
//        {
//            get => _isAspect4x3;
//            set
//            {
//                if (SetValue(ref _isAspect4x3, value))
//                {
//                    ViewModel.Renderer.AspectRatio = value ? Renderer.Aspect4x3 : Renderer.Aspect8x7;
//                }
//            }
//        }

//        public bool IsAspectRatioEnabled
//        {
//            get => IsStretched;
//        }

//        public bool AreAspectItemsEnabled
//        {
//            get => IsAspectPreserved && IsAspectRatioEnabled;
//        }

//        public Aspect[] Aspects =
//        {
//            Aspect.Native,
//            Aspect.Snes8x7,
//            Aspect.Tv4x3,
//            Aspect.Stretch
//        };

//        public

//        private Aspect _aspect;

//        private bool _isStretched;
//        private bool _isAspectPreserved;
//        private bool _isAspect4x3;
//    }
//}
