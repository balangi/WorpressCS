public class AdminBarViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserIsLoggedIn()) return Content("");

        return View("Default");
    }

    private bool UserIsLoggedIn()
    {
        // Replace with actual auth check
        return true;
    }
}