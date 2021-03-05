using System;

namespace SimpleWizard
{
    [Serializable]
    public class WizardException : Exception
    {
        public WizardException() { }
        public WizardException(string message) : base(message) { }
        public WizardException(string message, Exception inner) : base(message, inner) { }
        protected WizardException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

  



    


}
