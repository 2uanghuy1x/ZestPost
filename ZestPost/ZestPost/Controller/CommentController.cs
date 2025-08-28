using OpenQA.Selenium;
using System.Text.RegularExpressions;
using ZestPost.Base.Extension;
using ZestPost.Base.Facebook;

namespace ZestPost.Controllers
{
    public class CommentController
    {
        UtilitiesRequestFetchSync utilities_request = new UtilitiesRequestFetchSync();
        Random rd = new Random();
        public async Task<Result> CommentByProfile(object payload)
        {
            // Implementation for commenting by profile
            return Result.Ok("CommentByProfile successful");
        }

        public async Task<Result> CommentByPage(object payload)
        {
            // Implementation for commenting by page
            return Result.Ok("CommentByPage successful");
        }

        public async Task<Result> CommentByGroup(object payload)
        {
            // Implementation for commenting by group
            return Result.Ok("CommentByGroup successful");
        }

        public ViModelSync CommentPostForOneFriend(ChromeBrowser chromedriver, AccountFB acc, string uid, List<Article> lstPost, bool convert_text_to_image = false, bool reget_post = false, int num_comment = 0, int mintime = 0, int maxtime = 5, int timeupload = 2, bool openlink = false, CancellationToken token = default)
        {
            bool flag = false;
            string js = "";
            string data_result = null;
            Article post_cmt = null;
            int numsuccess = 0;
            bool send_content_success = false;
            ViModelSync result = new ViModelSync();
            Random rd = new Random();
            List<Article> lst_posti = HelperSync.CloneList(lstPost);
            List<string> lst_idpost = new List<string>();
            List<string> lst_src = new List<string>();
            try
            {
                List<PostDataSync> lst_post_uid = getListPostByUID(chromedriver, acc.Uid, uid, num_comment * 3, token);
                if (lst_post_uid.Count > 0)
                {
                    //foreach (Article post in lst_post_uid)
                    //{
                    //    lst_idpost.Add(post.title);
                    //}

                    if (token.IsCancellationRequested)
                        return result;

                    if (lst_idpost.Count() > 0)
                    {
                        num_comment = lst_idpost.Count < num_comment ? lst_idpost.Count : num_comment;
                        for (int i = 0; i < num_comment; i++)
                        {
                            string id_post = lst_idpost[rd.Next(0, lst_idpost.Count() - 1)];
                            lst_idpost.Remove(id_post);

                            if (reget_post && lst_posti.Count <= 0)
                            {
                                lst_posti = HelperSync.CloneList(lstPost);
                            }

                            if (lst_posti.Count > 0)
                            {
                                post_cmt = lst_posti[rd.Next(lst_posti.Count)];
                                //post_cmt.content = StringHelper.detechString(post_cmt.content);
                                lst_posti.Remove(post_cmt);
                            }

                            if (token.IsCancellationRequested)
                                break;

                            //string id_image = null;
                            //string pathTextImg = string.Format("{0}\\data\\virul_commentProfile_textImg.txt", Application.StartupPath);
                            //if (File.Exists(pathTextImg))
                            //{
                            //    string dataFile = File.ReadAllText(pathTextImg);
                            //    textImg = StringHelper.SpinContent(dataFile);
                            //}
                            //HelperTransTextToImg helper_translate = new HelperTransTextToImg();
                            //string path_image = string.Format("{0}Chromeprofile\\{1}\\image", Application.StartupPath, acc.Uid);
                            ////send image to comment
                            //if (convert_text_to_image)
                            //{
                            //    if (!Directory.Exists(path_image))
                            //    {
                            //        Directory.CreateDirectory(path_image);
                            //    }

                            //    path_image = string.Format("{0}Chromeprofile\\{1}\\image\\{2}.jpg", Application.StartupPath, acc.Uid, StringHelper.CreateRandomString(8, new Random()));
                            //    helper_translate.TransTextToImg(alignTextImg, textImg, path_image, fontTextImg, ImageFormat.Jpeg);
                            //    post_cmt = new Post { content = null, typepost = "media", picture = path_image, numpicture = 1 };
                            //}

                            //if (post_cmt.typepost == "media" && !string.IsNullOrEmpty(post_cmt.picture))
                            //{
                            //    lst_src = post_cmt.picture.Split('\n').ToList();
                            //    lst_src = HelperController.RemoveEmptyItems(lst_src);
                            //    foreach (string src in lst_src)
                            //    {
                            //        if (!File.Exists(src))
                            //        {
                            //            lst_src.Remove(src);
                            //        }
                            //    }
                            //    id_image = facebookService.UploadImageWithBase64(chromedriver, lst_src[rd.Next(0, lst_src.Count - 1)]);
                            //}
                            string id_image = "";
                            ViModelSync result_comment = PostCommentLinkAndImg(chromedriver, acc, id_post, post_cmt, token, id_image, 0);
                            if (result_comment.status)
                            {
                                numsuccess++;
                                result.status = true;
                                result.data = numsuccess.ToString();
                                //helperController.UpdateHistory(acc.Uid, "", "Bình luận trang cá nhân", "Bình luận vào tài khoản : " + uid, "", "comment_profile", "", 0, result_comment.data, 0);
                                //FillToDgvPost(acc.Uid, result_comment.data);
                                chromedriver.DelayTime(rd.Next(mintime, maxtime));
                            }

                            //if (File.Exists(path_image))
                            //{
                            //    helper_translate.DeleteImg(path_image);
                            //}
                        }
                    }
                    else
                    {
                        result.status = false;
                        result.message = HelperCore.L("msg594");
                        goto lb_finish;
                    }
                }
                else
                {
                    result.status = false;
                    result.message = HelperCore.L("msg594");
                    goto lb_finish;
                }

            lb_finish:;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }


        public ViModelSync PostCommentLinkAndImg(ChromeBrowser chromedriver, AccountFB acc, string id_post, Article post, CancellationToken token = default, string id_image = null, int type_comment = 0)
        {
            ViModelSync result = new ViModelSync();
            string content = post.Content;
            if (content.IsNotEmpty() && content.Contains("https"))
            {
                result = PostCommentElement(chromedriver, acc, id_post, post, token);
            }
            else
            {
                result = PostCommentHTTP(chromedriver, acc, id_post, post, token, id_image, type_comment);
            }

            return result;
        }

        public List<PostDataSync> getListPostByUID(ChromeBrowser chromedriver, string acc_uid, string uid, int totalpost = 5, CancellationToken token = default)
        {
            List<PostDataSync> lst_post = new List<PostDataSync>();
            try
            {
                chromedriver.GotoURL(string.Format("https://www.facebook.com/{0}", uid));
                chromedriver.DelayTime(1);
                chromedriver.ScrollPageRandom(rd.Next(200, 500), rd.Next(2, 6));

                string cursor = null;
                for (int l = 0; l < totalpost; l++)
                {
                    if (lst_post.Count() >= totalpost || token.IsCancellationRequested)
                        goto lb_finish;

                    string variables = $@"{{
                                            UFI2CommentsProvider_commentsKey:'ProfileCometTimelineRoute',
                                            afterTime:null,
                                            beforeTime:null,
                                            count: 3,
                                            cursor:'{cursor}',
                                            displayCommentsContextEnableComment:null,
                                            displayCommentsContextIsAdPreview:null,
                                            displayCommentsContextIsAggregatedShare:null,
                                            displayCommentsContextIsStorySet:null,
                                            displayCommentsFeedbackContext:null,
                                            feedLocation:'TIMELINE',
                                            feedbackSource:0,
                                            focusCommentID:null,
                                            memorializedSplitTimeFilter:null,
                                            omitPinnedPost:true,
                                            postedBy:null,
                                            privacy:null,
                                            privacySelectorRenderLocation:'COMET_STREAM',
                                            renderLocation:'timeline',
                                            scale:1,
                                            should_show_profile_pinned_post:true,
                                            stream_count:1,
                                            taggedInOnly:null,
                                            useDefaultActor:false,
                                            id:'{uid}',
                                            __relay_internal__pv__FBReelsEnableDeferrelayprovider:false
                                            }}";

                    string data_send = utilities_request.RenderDataSend(chromedriver, acc_uid, variables, "ProfileCometTimelineFeedRefetchQuery", "24809233985341850");
                    string html = utilities_request.RequestPost(chromedriver, "https://www.facebook.com/api/graphql/", data_send, chromedriver.GetURL());
                    if (!string.IsNullOrEmpty(html))
                    {
                        string data_get = html.Split('\n')[0];
                        var _json = JsonConvert.DeserializeObject<dynamic>(data_get);
                        cursor = _json.data.node?.timeline_list_feed_units.edges[0].cursor;
                        for (int i = 0; i < _json.data.node?.timeline_list_feed_units?.edges.Count; i++)
                        {
                            if (lst_post.Count() >= totalpost)
                                goto lb_finish;

                            if (token.IsCancellationRequested)
                            {
                                goto lb_finish;
                            }

                            var item = _json.data.node?.timeline_list_feed_units?.edges[i];
                            try
                            {
                                PostDataSync post = new PostDataSync();
                                post.title = item.node.post_id;
                                try
                                {
                                    post.content = item.node.comet_sections.content?.story?.message?.text;
                                }
                                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
                                post.timepost = item.node.comet_sections.context_layout?.story.comet_sections.metadata[0]?.story.creation_time;
                                post.link = item.node.comet_sections.context_layout?.story.comet_sections.metadata[0]?.story.url;
                                post.typepost = "status";
                                try
                                {
                                    List<string> _lstImage = new List<string>();
                                    string source_image = item.ToString();
                                    if (source_image.Contains("all_subattachments"))
                                    {
                                        for (int j = 0; j < item.node.comet_sections.content.story.attachments[0]?.styles.attachment.all_subattachments.nodes.Count; j++)
                                        {
                                            var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.all_subattachments.nodes[j];
                                            string link = item_link.media.image.uri;
                                            if (!string.IsNullOrEmpty(link))
                                                _lstImage.Add(link);
                                        }
                                    }
                                    else if (source_image.Contains("_photos_subattachments"))
                                    {
                                        if (source_image.Contains("five_photos_subattachments"))
                                        {
                                            for (int j = 0; j < item.node.comet_sections.content.story.attachments[0]?.styles.attachment.five_photos_subattachments.nodes.Count; j++)
                                            {
                                                var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.five_photos_subattachments.nodes[j];
                                                string link = item_link.media.image.uri;
                                                if (!string.IsNullOrEmpty(link))
                                                    _lstImage.Add(link);
                                            }
                                        }
                                        else if (source_image.Contains("four_photos_subattachments"))
                                        {
                                            for (int j = 0; j < item.node.comet_sections.content.story.attachments[0]?.styles.attachment.four_photos_subattachments.nodes.Count; j++)
                                            {
                                                var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.four_photos_subattachments.nodes[j];
                                                string link = item_link.media.image.uri;
                                                if (!string.IsNullOrEmpty(link))
                                                    _lstImage.Add(link);
                                            }
                                        }
                                        else if (source_image.Contains("three_photos_subattachments"))
                                        {
                                            for (int j = 0; j < item.node.comet_sections.content.story.attachments[0]?.styles.attachment.three_photos_subattachments.nodes.Count; j++)
                                            {
                                                var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.three_photos_subattachments.nodes[j];
                                                string link = item_link.media.image.uri;
                                                if (!string.IsNullOrEmpty(link))
                                                    _lstImage.Add(link);
                                            }
                                        }
                                        else if (source_image.Contains("two_photos_subattachments"))
                                        {
                                            for (int j = 0; j < item.node.comet_sections.content.story.attachments[0]?.styles.attachment.two_photos_subattachments.nodes.Count; j++)
                                            {
                                                var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.two_photos_subattachments.nodes[j];
                                                string link = item_link.media.image.uri;
                                                if (!string.IsNullOrEmpty(link))
                                                    _lstImage.Add(link);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var item_link = item.node.comet_sections.content.story.attachments[0]?.styles.attachment.media;
                                        string link = item_link.photo_image.uri;
                                        if (!string.IsNullOrEmpty(link))
                                            _lstImage.Add(link);
                                    }

                                    if (_lstImage.Count() > 0)
                                    {
                                        post.picture = string.Join("\n", _lstImage);
                                        post.numpicture = _lstImage.Count();
                                        post.typepost = "media";
                                    }
                                }
                                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
                                lst_post.Add(post);
                            }
                            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
                        }
                        chromedriver.ScrollPage(rd.Next(100, 300));
                    }
                    else
                    {
                        break;
                    }
                }
            lb_finish:;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return lst_post;
        }

        public ViModelSync PostCommentElement(ChromeBrowser chromedriver, AccountFB acc, string id_post, Article post, CancellationToken token = default)
        {
            ViModelSync result = new ViModelSync();
            UtilitiesRequestFetchSync utilities_request = new UtilitiesRequestFetchSync();
            string content = post.Content;
            bool flag = false;
            bool send_content_success = false;
            List<string> lstNode = new List<string>();
            string node = "";

            try
            {
                chromedriver.GotoURL("https://facebook.com/" + id_post);

                chromedriver.DelayRandom(1, 3);
                lstNode = new List<string> { "//form[@role=\"presentation\"]//div[@role=\"textbox\"]", "//div[contains(@aria-label,\"Bình luận dưới\")]/p", "//div[contains(@aria-label,\"Comment as\")]/p", "//div[contains(@aria-label,\"Viết bình\")]/p", "//div[contains(@aria-label,\"Write a\")]/p", };
                node = chromedriver.GetElementExistFromList(3, 1, lstNode);
                if (!string.IsNullOrWhiteSpace(node))
                {
                    chromedriver.Click(SelectorType.ByXPath, node);
                }
                if (post.LinkImg.Count > 0)
                {
                    List<string> lst_pathimg = post.LinkImg;
                    if (lst_pathimg.Count() > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return result;
                        }
                        node = "[type=\"file\"]";
                        flag = chromedriver.IsExistElement(SelectorType.ByCssSelector, node).IsOk();
                        if (flag)
                        {
                            int index = chromedriver.CountElement(4, 2, node);
                            send_content_success = chromedriver.SendKeys(SelectorType.ByCssSelector, node, lst_pathimg[Random.Shared.Next(lst_pathimg.Count)], index: index - 1).IsOk();
                            chromedriver.DelayTime(2);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(post.Content))
                {
                    node = chromedriver.GetElementExistFromList(3, 1, lstNode);
                    if (!string.IsNullOrWhiteSpace(node))
                    {
                        //lock (sync_data)
                        //{
                        int index = chromedriver.CountElement(3, 2, node);
                        //send_content_success = HelperController.CopyToClipboardAndSendkeys(chromedriver, 3, node, index - 1, content);
                        chromedriver.DelayTime(1);
                        if (!send_content_success)
                        {
                            send_content_success = chromedriver.SendKeys(SelectorType.ByXPath, node, content).IsOk();
                        }
                        //}
                    }
                }

                if (send_content_success)
                {
                    int index = chromedriver.CountElement(3, 2, node);
                    flag = chromedriver.SendKeys(SelectorType.ByXPath, node, Keys.Enter, index: index - 1).IsOk();
                    if (flag)
                    {
                        result.status = true;
                        result.data = "https://facebook.com/" + id_post;
                        result.message = HelperCore.L("msg129");
                    }
                }
                else
                {
                    result.status = false;
                    result.message = HelperCore.L("msg597");
                }

            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }

        public ViModelSync PostCommentHTTP(ChromeBrowser chromedriver, AccountFB acc, string id_post, Article post, CancellationToken token = default, string id_image = null, int type_comment = 0)
        {
            ViModelSync result = new ViModelSync();
            UtilitiesRequestFetchSync utilities_request = new UtilitiesRequestFetchSync();
            Random rd = new Random();
            string content = post.Content;
            chromedriver.GotoURL("https://facebook.com/" + id_post);
            string checkUrl = chromedriver.GetURL();
            int startIndex = checkUrl.IndexOf("//") + 2; // Tìm vị trí bắt đầu sau "//"
            int endIndex = checkUrl.IndexOf('.', startIndex); // Tìm vị trí dấu "." tiếp theo
            string domainName = checkUrl.Substring(startIndex, endIndex - startIndex);
            string apiFb = "https://www.facebook.com/api/graphql/";
            string graphglFb = apiFb.Replace("www", domainName);
            chromedriver.DelayRandom(1, 3);
            try
            {
                string variables = $@"{{
                                      feedLocation: 'GROUP',
                                      feedbackSource: 0,
                                      groupID: null,
                                      input: {{
                                        client_mutation_id: '1',
                                        actor_id: '{acc.Uid}',
                                        attachments: null,
                                        feedback_id: '{HelperSync.FeedbackID(id_post)}',
                                        formatting_style: null,
                                        message: {{
                                          ranges: [],
                                          text: '{HelperStringSync.CreateRandomString(rd.Next(8, 15), rd) + "♘☂♉"}'
                                        }},
                                        vod_video_timestamp: null,
                                        feedback_referrer: '/',
                                        is_tracking_encrypted: true,
                                        tracking: [null],
                                        feedback_source:'PROFILE',
                                        idempotence_token: 'client:{Guid.NewGuid().ToString()}',
                                        session_id: '{Guid.NewGuid().ToString()}'
                                      }},
                                      inviteShortLinkKey: null,
                                      renderLocation: null,
                                      scale: 1,
                                      useDefaultActor: false,
                                      focusCommentID: null,
                                      __relay_internal__pv__IsWorkUserrelayprovider: false
                                    }}";

                string data_send = utilities_request.RenderDataSend(chromedriver, acc.Uid, variables, "useCometUFICreateCommentMutation", "9271678509515893");
                string create_comment = utilities_request.RequestPost(chromedriver, graphglFb, data_send, chromedriver.GetURL());
                if (string.IsNullOrEmpty(create_comment))
                    goto lb_finish;

                var _json = JsonConvert.DeserializeObject<dynamic>(create_comment);
                string id_comment = _json.data.comment_create?.feedback_comment_edge?.node.id;
                string link_comment = _json.data.comment_create?.feedback_comment_edge?.node.url;
                if (!string.IsNullOrEmpty(id_comment))
                {
                    string entity = "";
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = content.Replace("\r\n", "\\n").Replace("\"", "'");
                        if (content.Contains("@["))
                        {
                            try
                            {
                                string pattern = @"\@\[(\d+)\]";
                                MatchCollection matches = Regex.Matches(content, pattern);

                                int index_tag = 1;
                                foreach (System.Text.RegularExpressions.Match match in matches)
                                {
                                    string uid = match.Groups[1].Value;
                                    content = content.Replace(match.Value, uid);
                                    int offset = content.IndexOf(uid);
                                    string content_clone = content;
                                    string spli_text = content_clone.Substring(0, offset);
                                    int count = 1;
                                    int index = 0;
                                    while ((index = spli_text.IndexOf("\\n", index + 1)) != -1)
                                    {
                                        count++;
                                    }

                                    // Lấy giá trị UID từ nhóm trong ngoặc đơn
                                    string entity_tag = " {\"entity\": {\"id\": \"" + uid + "\"},\"length\": 0,\"offset\": " + (offset - count - index_tag) + "},";
                                    entity = entity + entity_tag;
                                    content = content.Replace(uid, "");
                                    index_tag++;
                                }
                                entity = entity.TrimEnd(',');
                            }
                            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
                        }
                    }
                    else
                    {
                        content = "";
                    }

                    //upload image
                    string attach = "null";
                    if (post.LinkImg.Count > 0 && !string.IsNullOrEmpty(id_image))
                    {
                        attach = $@"[{{media: {{id: '{id_image}'}}}}]";
                    }

                    //edit comment
                    variables = $@"{{
                                          input: {{
                                            client_mutation_id: '2',
                                            actor_id: '{acc.Uid}',
                                            attachments: {attach},
                                            comment_id: '{id_comment}',
                                            formatting_style: 'PLAIN_TEXT',
                                            message: {{
                                              ranges: [{entity}],
                                              text: '`{content}`'
                                            }},
                                            tracking: [null]
                                          }},
                                          feedLocation: 'DEDICATED_COMMENTING_SURFACE',
                                          scale: 1,
                                          useDefaultActor: false,
                                          __relay_internal__pv__IsWorkUserrelayprovider: false
                                        }}
                                        ";
                    data_send = utilities_request.RenderDataSend(chromedriver, acc.Uid, variables, "useCometUFIEditCommentMutation", "8999896553402228");
                    create_comment = utilities_request.RequestPost(chromedriver, graphglFb, data_send, chromedriver.GetURL());
                    if (!string.IsNullOrEmpty(create_comment) && create_comment.Contains("legacy_fbid"))
                    {
                        result.status = true;
                        result.data = "https://www.facebook.com/" + HelperStringSync.getFromIndex(create_comment, create_comment.IndexOf("legacy_fbid\":\"") + 14, "\"");
                        result.message = HelperCore.L("msg598");

                        //chromedriver.SendEnter(3, , 0, 0, null, 0);
                        //chromedriver.DelayRandom(1, 2);

                        //result.message = HelperCore.L("msg129");
                    }
                    else
                    {
                        result.status = false;
                        result.message = HelperCore.L("msg597");
                        if (type_comment == 0)
                        {
                            //comment profile
                            variables = $@"{{
                                    groupID: null,
                                    input: 
                                        {{
                                            client_mutation_id: '1',
                                            actor_id: '{acc.Uid}',
                                            attribution_id_v2:'CometSinglePostDialogRoot.react,comet.post.single_dialog,via_cold_start,1731685484187,867375,,,',
                                            comment_id: '{id_comment}',
                                            remove_location: 'MENU',
                                            tracking: [],
                                        }},
                                    inviteShortLinkKey: null,
                                    renderLocation: null,
                                    scale: 1,
                                    }}";
                        }
                        else
                        {
                            //comment group
                            variables = $@"{{
                                    groupID: null,
                                    input: 
                                        {{
                                            client_mutation_id: '1',
                                            actor_id: '{acc.Uid}',
                                            attribution_id_v2:'CometGroupPermalinkRoot.react,comet.group.permalink,unexpected,1731684625772,312547,2361831622,,;CometActivityLogMainContentRoot.react,comet.ActivityLog.CometActivityLogMainContentRoute,unexpected,1731684612498,20061,,,;CometActivityLogMainContentRoot.react,comet.ActivityLog.CometActivityLogMainContentRoute,unexpected,1731684609676,867379,,,;ProfileCometTimelineListViewRoot.react,comet.profile.timeline.list,unexpected,1731684606012,381740,190055527696468,,;CometHomeRoot.react,comet.home,via_cold_start,1731684595838,91635,4748854339,,',
                                            comment_id: '{id_comment}',
                                            remove_location: 'MENU',
                                            tracking: [],
                                        }},
                                    inviteShortLinkKey: null,
                                    renderLocation: null,
                                    scale: 1,
                                    }}";
                        }
                        data_send = utilities_request.RenderDataSend(chromedriver, acc.Uid, variables, "useCometUFIDeleteCommentMutation", "8724763607600242");
                        create_comment = utilities_request.RequestPost(chromedriver, graphglFb, data_send, chromedriver.GetURL());
                    }
                }
            lb_finish:;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }
    }
}
