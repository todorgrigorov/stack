# Setup
To integrate the framework with a WEB app (or MOBILE app with Xamarin) call ```StackConfiguration.Setup()``` to configure all internals.

IOC container can be set-up by calling ```ContainerConfiguration.Setup(...)```.

The consumer app must also include an implementation of ```IApplication``` interface in its startup project.
