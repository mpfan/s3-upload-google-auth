using API.Messaging.Requests;
using FluentValidation;

namespace API.Validators;

public class PutFileRequestValidator : AbstractValidator<PutFileRequest> 
{
    public PutFileRequestValidator()
    {
        RuleFor(r => r.Key).NotEmpty();
    }
}