using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace Stocks.UI.Services {
    public class PageLocator {
        private static PageLocator instance;

        Dictionary<Type, BuilderRegister<Page>> builderRegisters = new Dictionary<Type, BuilderRegister<Page>>();

        public static PageLocator Instance {
            get {
                if (instance == null) {
                    instance = new PageLocator();
                }
                return instance;
            }
        }

        private PageLocator() { }

        public void Register<ViewModelType, PageType>(string tag = null) where PageType: Page {
            Type viewModelType = typeof(ViewModelType);
            Type pageType = typeof(PageType);
            BuilderRegister<Page> register;
            if (!builderRegisters.TryGetValue(viewModelType, out register)) {
                register = new BuilderRegister<Page>();
                builderRegisters.Add(viewModelType, register);
            }
            register[tag] = GetPageFactory(pageType);
        }

        public void Register<ViewModelType>(Func<object, Page> factory, string tag = null) {
            Type viewModelType = typeof(ViewModelType);
            BuilderRegister<Page> register;
            if (!builderRegisters.TryGetValue(viewModelType, out register)) {
                register = new BuilderRegister<Page>();
                builderRegisters.Add(viewModelType, register);
            }
            register[tag] = factory;
        }

        public Page GetPage(object viewModel, string tag = null) {
            Type viewModelType = viewModel.GetType();
            BuilderRegister<Page> pageBuilderRegister;
            Func<object, Page> factoryMethod;
            if (!builderRegisters.TryGetValue(viewModelType, out pageBuilderRegister)) return null;
            if (!pageBuilderRegister.TryGetValue(tag, out factoryMethod)) return null;
            Page page = factoryMethod.Invoke(viewModel);
            return page;
        }

        Func<object, Page> GetPageFactory(Type pageType) {
            ConstructorInfo defaultCtor = pageType.GetConstructor(new Type[0]);
            return (vm) => {
                Page page = (Page)defaultCtor.Invoke(new object[0]);
                page.BindingContext = vm;
                return page;
            };
        }
    }

    class BuilderRegister<T> {
        Dictionary<string, Func<object, T>> builders = new Dictionary<string, Func<object, T>>();
        Func<object, T> defaultBuilder;

        public Func<object, T> this[string key] {
            get {
                Func<object, T> builder;
                if(!builders.TryGetValue(key, out builder)) {
                    builder = defaultBuilder;
                }
                return builder;
            }
            set {
                if (key == null) {
                    defaultBuilder = value;
                } else {
                    builders[key] = value;
                }
            }
        }

        public bool TryGetValue(string key, out Func<object, T> builder) {
            if (key == null) {
                builder = defaultBuilder;
            } else if (!builders.TryGetValue(key, out builder)) {
                builder = defaultBuilder;
            }
            return builder != null;
        }
    }
}
