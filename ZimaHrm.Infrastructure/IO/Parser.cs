namespace ZimaHrm.Core.Infrastructure.IO
{
	internal interface Parser {
		ParsedLine Parse( string line );
	}
}
