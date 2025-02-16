namespace Common;

public static class StepUtilities
{
    public static void Step(int stepNumber, string description, Action action, int afterDelayMs = 0)
    {
        try
        {
            action();
            Console.WriteLine($"[✓] Step {stepNumber}: {description}");
            if (afterDelayMs > 0)
            {
                Thread.Sleep(afterDelayMs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[x] Step {stepNumber} FAILED: {description}. Error => {ex.Message}");
            throw;
        }
    }
    
    public static void Step(int stepNumber, string description, Func<bool> function, string errorMessage = "", int afterDelayMs = 0)
    {
        try
        {
            Console.WriteLine(function()
                ? $"[✓] Step {stepNumber}: {description}"
                : $"[x] Step {stepNumber} FAILED: {description}. Error => {(string.IsNullOrWhiteSpace(errorMessage) ? "No error provided" : errorMessage)}");

            if (afterDelayMs > 0)
            {
                Thread.Sleep(afterDelayMs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[x] Step {stepNumber} FAILED: {description}. Error => {ex.Message}");
            throw;
        }
    }
}