using Justwish.Users.Domain;
using Microsoft.Extensions.Options;

namespace Justwish.Users.Application;

public sealed class DefaultVerificationCodeGenerator : IVerificationCodeGenerator
{
    private static readonly Random Random = new();

    private readonly EmailVerificationOptions _options;

    public DefaultVerificationCodeGenerator(IOptions<EmailVerificationOptions> options)
    {
        _options = options.Value;
    }
    
    public int GenerateCode()
    {
        int lowerBound = Pow(10, _options.CodeLength - 1);
        int upperBound = Pow(10, _options.CodeLength);
        
        int code = Random.Next(lowerBound, upperBound);
        return code;

        static int Pow(int n, int k)
        {
            int result = 1;
            for (int i = 0; i < k; i++)
            {
                result *= n;
            }
            return result;
        }
    }
}