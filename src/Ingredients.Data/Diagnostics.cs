using System.Diagnostics;

namespace Ingredients.Data;

public class Diagnostics
{
    private static readonly ActivitySource Activities = new ActivitySource("Ingredients.Data");

    public static Activity? StartActivity(string name) => Activities.StartActivity(name);
}