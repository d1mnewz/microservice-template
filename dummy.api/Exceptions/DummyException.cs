using System;

namespace dummy.api.Exceptions
{
    /// <summary>
    /// Dummy Exception.
    /// </summary>
    [Serializable]
    public class DummyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyException"/> class.
        /// </summary>
        public DummyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyException"/> class.
        /// </summary>
        /// <param name="message">Exception details.</param>
        public DummyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyException"/> class.
        /// </summary>
        /// <param name="message">Exception details.</param>
        /// <param name="ex">Inner exception.</param>
        public DummyException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}