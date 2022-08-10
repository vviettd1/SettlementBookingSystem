using System;
using FluentValidation;

namespace SettlementBookingSystem.Application.Bookings.Commands
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(b => b.Name).NotEmpty();
            RuleFor(b => b.BookingTime).Matches("[0-9]{1,2}:[0-9][0-9]")
                .Must(c => TimeSpan.Parse(c) >= new TimeSpan(9, 0, 0) &&
                           TimeSpan.Parse(c) <= new TimeSpan(16, 0, 0));
        }
    }
}
