using CommandLine;

namespace Migrations.DomainModels
{
    public class CommandLineArgsOptions
    {
        /// <summary>
        /// Command line argument Down.
        /// </summary>
        [Option('u', "up", Required = false, HelpText = "Migrate up.")]
        public bool Up { get; set; }

        /// <summary>
        /// Command line argument Down.
        /// </summary>
        [Option('d', "down", Required = false, HelpText = "Migrate down.")]
        public bool Down { get; set; }
    }
}
