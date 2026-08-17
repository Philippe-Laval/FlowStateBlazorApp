namespace FlowStateBlazorApp.Library.Nodes;

using FlowState.Attributes;
using FlowState.Models.Execution;

/// <summary>
/// Node that multiply two numbers together
/// </summary>
[FlowNodeMetadata(
    Category = "Math",
    Title = "Divide",
    Description = "Divide two numbers together",
    Icon = "➗",
    Order = 1)]
public partial class DivideNode : ExecutableNodeBase
{
    private float inputA = 0;
    private float inputB = 0;

    public override async ValueTask ExecuteAsync(FlowExecutionContext context)
    {
        await ExecuteWithProgressAsync((ctx) =>
        {
            // Get input values using context API
            // Note: GetInputSocketData<float> will handle conversion from long/int to float automatically
            inputA = ctx.GetInputSocketData<float>("InputA");
            inputB = ctx.GetInputSocketData<float>("InputB");

            // https://en.wikipedia.org/wiki/IEEE_754
            float result;

            // Calculate division
            if (inputB != 0)
            {
                if (float.IsNegativeInfinity(inputB))
                {
                    if (inputA > 0)
                        result = float.NegativeZero;
                    else
                        result = 0;
                }
                else if (float.IsInfinity(inputB))
                {
                    if (inputA > 0)
                        result = 0;
                    else
                        result = float.NegativeZero;
                }
                else
                {
                    result = inputA / inputB;
                }
            }
            else
            {
                // inputB is 0 (or -0 : float.NegativeZero)

                if (inputA == 0)
                {
                    // 0 / 0 = NaN
                    result = float.NaN;
                }
                else if (float.IsNegative(inputB))
                {
                    if (inputA > 0)
                        result = float.NegativeInfinity;
                    else
                        result = float.PositiveInfinity;
                }
                else
                {
                    if (inputA > 0)
                        result = float.PositiveInfinity;
                    else
                        result = float.NegativeInfinity;
                }
            }

            // Set output using context API
            ctx.SetOutputSocketData("Output", result);

            return ValueTask.CompletedTask;
        }, context);
    }
}
