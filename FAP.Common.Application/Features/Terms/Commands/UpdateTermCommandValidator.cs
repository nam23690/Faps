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
    public class UpdateTermCommandValidator : AbstractValidator<UpdateTermCommand>
    {
        private readonly ITermRepository _termReporitory;

        public UpdateTermCommandValidator(ITermRepository termReporitory)
        {

            _termReporitory = termReporitory;

            
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Startdate không được để trống")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Startdate phải lớn hơn hoặc bằng ngày hiện tại");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate không được để trống")
                 .GreaterThan(x => x.StartDate).WithMessage("EndDate phải lớn hơn StartDate")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("EndDate phải lớn hơn hoặc bằng ngày hiện tại");
            RuleFor(x => x.SemesterName)
                .NotEmpty().WithMessage("SemesterName không được để trống")
                .MaximumLength(100).WithMessage("SemesterName không được vượt quá 100 ký tự")
                ; 
        }
        
    }
}


   

