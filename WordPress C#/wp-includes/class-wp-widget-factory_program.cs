services.AddTransient<RecentPostsWidget>();
services.AddSingleton<IWidgetFactory, WidgetFactory>(provider =>
{
    var factory = new WidgetFactory(provider);

    // ثبت تمام ویجت‌های موجود
    factory.Register<RecentPostsWidget>();
    factory.Register<SearchWidget>();
    factory.Register<CustomMenuWidget>();

    return factory;
});