using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WARTLS.XMPP.QUERY;

namespace WARTLS
{
    class MessageFactory
    {
        internal Dictionary<string,Type> Packets;
        internal MessageFactory()
        {
            Packets = new Dictionary<string, Type>()
            {
                {"account",typeof(Account) },
                {"get_master_server",typeof(GetMasterServer) },
                {"get_account_profiles",typeof(GetAccountProfiles) },
                {"items",typeof(GameResourcesGet) },
                {"shop_get_offers",typeof(GameResourcesGet) },
                {"get_configs",typeof(GameResourcesGet) },
                {"quickplay_maplist",typeof(GameResourcesGet) },
                {"missions_get_list",typeof(GameResourcesGet) },
                {"join_channel",typeof(ChannelOperation) },
                {"create_profile",typeof(ChannelOperation) },
                {"switch_channel",typeof(ChannelOperation) },
                {"persistent_settings_get",typeof(PersistentSettingsGet) },
                {"shop_buy_offer",typeof(ShopBuyOffer) },
                {"shop_buy_multiple_offer",typeof(ShopBuyOffer) },
                {"extend_item",typeof(ShopBuyOffer) },
                {"setcharacter",typeof(SetCharacter) },
                {"setcurrentclass",typeof(SetCurrentClass) },
                {"persistent_settings_set",typeof(PersistentSettingsSet) },
                {"resync_profile",typeof(ResyncProfile) },
                {"get_player_stats",typeof(GetPlayerStats) },
                {"get_achievements",typeof(GetAchievements) },
                {"clan_list",typeof(GetClans) },
                {"sync_notification",typeof(SyncNotification) },
                {"get_profile_performance",typeof(GetProfilePerformance) },
                {"player_status",typeof(PlayerStatus) },
                {"get_master_servers",typeof(Account) },
                {"channel_logout",typeof(ChannelLogout) },
                {"tutorial_status",typeof(TutorialStatus) },
                {"get_expired_items",typeof(GetExpiredItems) },
                {"confirm_notification",typeof(ConfirmNotification) },
                {"send_invitation",typeof(SendInvitation) },
                {"friend_list",typeof(FriendList) },
                {"remove_friend",typeof(RemoveFriend) },
                {"get_last_seen_date",typeof(GetLastSeenDate) },
                {"get_contracts",typeof(GetContacts) },
                {"data",typeof(CompressedQuery) },
                {"peer_player_info",typeof(ToOnlinePlayers) },
                {"peer_status_update",typeof(ToOnlinePlayers) },
                {"p2p_ping",typeof(ToOnlinePlayers) },
                {"preinvite_cancel",typeof(ToOnlinePlayers) },
                {"preinvite_invite",typeof(ToOnlinePlayers) },
                {"follow_send",typeof(ToOnlinePlayers) },
                {"gameroom_open",typeof(GameRoom_Open) },
                {"gameroom_setplayer",typeof(GameRoom_SetPlayer) },
                {"gameroom_update_pvp",typeof(GameRoom_UpdatePvP) },
                {"gameroom_askserver",typeof(GameRoom_AskServer) },
                {"gameroom_setname",typeof(GameRoom_SetName) },
                {"gameroom_setinfo",typeof(GameRoom_SetInfo) },
                {"gameroom_sync",typeof(GameRoom_Sync) },
                {"gameroom_leave",typeof(GameRoom_Leave) },
                {"gameroom_promote_to_host",typeof(GameRoom_PromoteToHost) },
                {"gameroom_kick",typeof(GameRoom_Kick) },

                {"invitation_request",typeof(InvitationRequest) },
                {"invitation_result",typeof(InvitationResult) },
                {"invitation_send",typeof(InvitationSend) },
                {"invitation_accept",typeof(InvitationAccept) },
                {"clan_create",typeof(ClanCreate) },

                {"session_join",typeof(SessionJoin) },

                {"broadcast_session_result",typeof(BroadcastSessionResults) },
                {"gameroom_get",typeof(GameRoom_Get) },
                {"gameroom_join",typeof(GameRoom_Join) },
                {"message",typeof(Messages) },
                {"lobbychat_getchannelid",typeof(LobbychatGetChannelId) },
                {"profile_info_get_status",typeof(ProfileItemGetStatus) },
                {"sync_notifications",typeof(SyncNotification) },

            };

            Console.WriteLine($"[{this.GetType().Name}] Loaded {Packets.Count} XMPP queries (stanza's)");
            Console.ResetColor();
        }
    }
}
