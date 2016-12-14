using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Extensions
{
    public static class ChannelRepositoryExtension
    {
        public static async Task UpdateUsernameAsync(this IRepositotory<Channel> channels, string userId, string newUsername)
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, new ObjectId(userId));
            var update = Builders<Channel>.Update.Set(c => c.Username, newUsername);
            await channels.Collection.UpdateOneAsync(filter, update);
        }

        public static async Task UpdateAvatarAsync(this IRepositotory<Channel> channels, string userId, string newAvatar)
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, new ObjectId(userId));
            var update = Builders<Channel>.Update.Set(c => c.AvatarUri, newAvatar);
            await channels.Collection.UpdateOneAsync(filter, update);
        }
    }
}
