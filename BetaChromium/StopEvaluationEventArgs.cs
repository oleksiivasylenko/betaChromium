using System;

namespace BetaChromium
{
    public class StopEvaluationEventArgs
    {
        public void StopExecution()
        {
            throw new StopEvaluationException("Break Execution");
        }
    }

    public class StopEvaluationException: Exception
    {
        public StopEvaluationException() : base()
        {
        }

        public StopEvaluationException(string message) : base(message)
        {
        }

        public StopEvaluationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}