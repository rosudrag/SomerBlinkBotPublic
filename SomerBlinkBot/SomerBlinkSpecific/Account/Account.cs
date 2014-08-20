namespace SomerBlinkSpecific.Account
{
    /// <summary>
    /// Somer Blink Account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the isk.
        /// </summary>
        /// <value>
        /// The isk.
        /// </value>
        public long Isk { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        /// <value>
        /// The tokens.
        /// </value>
        public long Tokens { get; set; }
    }
}
