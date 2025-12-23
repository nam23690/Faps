using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FAP.Common.Application.Features.Term.Commands
{
    public class CreateTermCommandValidator : AbstractValidator<CreateTermCommand>
    {
        private readonly ITermRepository _termRepository;

        public CreateTermCommandValidator(ITermRepository termRepository)
        {

            _termRepository = termRepository;

            // Username: required, min length, allowed chars, unique
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("EndDate không được để trống")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("EndDate phải lớn hơn hoặc bằng ngày hiện tại");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate không được để trống")
                 .GreaterThan(x => x.StartDate).WithMessage("EndDate phải lớn hơn StartDate")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("EndDate phải lớn hơn hoặc bằng ngày hiện tại");
            RuleFor(x => x.SemesterName)
                .NotEmpty().WithMessage("SemesterName không được để trống")
                .MaximumLength(100).WithMessage("SemesterName không được vượt quá 100 ký tự");
                //.MustAsync(BeUniqueName).WithMessage("Termname already exists."); 
            
            
        }


        
    }
}


   

