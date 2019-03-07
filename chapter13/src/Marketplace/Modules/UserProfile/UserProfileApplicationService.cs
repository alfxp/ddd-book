using System;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Domain.UserProfile;
using Marketplace.EventSourcing;

namespace Marketplace.Modules.UserProfile
{
    public class UserProfileApplicationService : 
        ApplicationService<Ads.Domain.UserProfile.UserProfile, UserId>
    {
        public UserProfileApplicationService(
            IAggregateStore store,
            CheckTextForProfanity checkText) : base(store)
        {
            var checkText1 = checkText;
            
            CreateWhen<Contracts.V1.RegisterUser>(
                cmd => new UserId(cmd.UserId),
                (cmd, id) => new Ads.Domain.UserProfile.UserProfile(
                    id, FullName.FromString(cmd.FullName), 
                    DisplayName.FromString(cmd.DisplayName, checkText1))
                );
            
            UpdateWhen<Contracts.V1.UpdateUserFullName>(
                cmd => new UserId(cmd.UserId), 
                (user, cmd) => user.UpdateFullName(FullName.FromString(cmd.FullName)));
            
            UpdateWhen<Contracts.V1.UpdateUserDisplayName>(
                cmd => new UserId(cmd.UserId), 
                (user, cmd) => user.UpdateDisplayName(
                    DisplayName.FromString(cmd.DisplayName, checkText1)));
            
            UpdateWhen<Contracts.V1.UpdateUserProfilePhoto>(
                cmd => new UserId(cmd.UserId), 
                (user, cmd) => user.UpdateProfilePhoto(new Uri(cmd.PhotoUrl)));
        }
    }
}