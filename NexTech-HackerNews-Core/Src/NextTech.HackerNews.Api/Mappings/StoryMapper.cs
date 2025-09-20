namespace NexTech.HackerNews.Api.Mappings
{
    using AutoMapper;
    using Nextech.HackerNews.Application.Entities;
    using NexTech.HackerNews.Api.Models;

    /// <summary>
    /// Defines the <see cref="StoryMapper" />
    /// </summary>
    public class StoryMapper : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoryMapper"/> class.
        /// </summary>
        public StoryMapper()
        {
            CreateMap<HackerNewsStory, StoryResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));
        }
    }
}
