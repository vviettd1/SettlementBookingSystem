using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SettlementBookingSystem.Application.Bookings.Dtos;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using SettlementBookingSystem.Application.Exceptions;

namespace SettlementBookingSystem.Application.Bookings.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
    {
        private readonly TimeSpan _startDate = new TimeSpan(9, 0, 0);
        private readonly TimeSpan _endDate = new TimeSpan(16, 0, 0);
        private readonly string _cacheKey = "bookings";

        private readonly IMemoryCache _memoryCache;

        public CreateBookingCommandHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var response = new BookingDto();

            if (_memoryCache.TryGetValue(_cacheKey, out List<TimeSpan?> bookingTimes))
            {
                if (bookingTimes.Count==3)
                {
                    throw new ConflictException("Maximum booking is 4");
                }
            }

            bookingTimes ??= new List<TimeSpan?>();

            var bookingTime = TimeSpan.Parse(request.BookingTime);
            if (bookingTime<_startDate || bookingTime>_endDate)
            {
                throw new ValidationException("Time is invalid");
            }

            var existTime =
                bookingTimes.FirstOrDefault(c => c <= bookingTime && c.Value.Add(new TimeSpan(1, 0, 0)) > bookingTime);
            if (existTime!=null)
            {
                throw new ConflictException("Conflict time");
            }
            bookingTimes.Add(bookingTime);
            _memoryCache.Set(_cacheKey, bookingTimes);
            return await Task.FromResult(response);

        }
    }
}
