using System.Diagnostics.CodeAnalysis;

namespace MRoessler.BlazorBottomSheet.Sample.Maui;

[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "Sample.Maui" };
	}
}
