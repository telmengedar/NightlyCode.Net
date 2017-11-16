namespace NightlyCode.Net.Browser {

    /// <summary>
    /// parameter for web requests
    /// </summary>
    public class Parameter {

        /// <summary>
        /// creates a new parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Parameter(string name, string value) {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// name of parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// value of parameter
        /// </summary>
        public string Value { get; } 
    }
}