using MediatR;
using HomeTech.Application.DTOs;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.CQRS.Handlers
{
    public class GetAllHomeTechServicesQueryHandler :
        IRequestHandler<GetAllServicesQuery, List<ServicesDto>>
    {
        private readonly IHomeTechServiceRepository _repository;

        public GetAllHomeTechServicesQueryHandler(IHomeTechServiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ServicesDto>> Handle(GetAllServicesQuery query, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(cancellationToken);

            return result.Select(r => new ServicesDto(r.Id, r.Name.Trim(), r.Description.Trim())).ToList();
        }
    }
}