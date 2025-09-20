import React, { useState, useEffect, useCallback } from 'react';
import './PostPersonal.css';
import { csharpApi, fetchAccounts, fetchCategories } from '../../api';
import ArticleManagement from '../../ArticleManagement'; // Import the ArticleManagement component

const PostPersonal = () => {
    const [isArticleManagementOpen, setIsArticleManagementOpen] = useState(false);
    const [selectedArticles, setSelectedArticles] = useState([]);

    // Account management states
    const [accounts, setAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [selectedAccountIds, setSelectedAccountIds] = useState(new Set());

    // Configuration states
    const [concurrentThreads, setConcurrentThreads] = useState(1);
    const [maxPostsPerAccount, setMaxPostsPerAccount] = useState(0);
    const [switchAccountOnError, setSwitchAccountOnError] = useState(1);
    const [switchAccountAfterPost, setSwitchAccountAfterPost] = useState(1);
    const [minPostInterval, setMinPostInterval] = useState(1);
    const [maxPostInterval, setMaxPostInterval] = useState(1);

    const [commonConfig, setCommonConfig] = useState(true);
    const [postOrder, setPostOrder] = useState('random'); // 'random' or 'sequential'
    const [noDuplicatePosts, setNoDuplicatePosts] = useState(true);
    const [imageUploadWaitTime, setImageUploadWaitTime] = useState(1);
    const [includeBackgroundImage, setIncludeBackgroundImage] = useState(true);
    const [commentAfterPost, setCommentAfterPost] = useState(true);
    const [commentContent, setCommentContent] = useState('');

    const loadAccounts = useCallback(async () => {
        try {
            const data = await fetchAccounts();
            let filteredAccounts = data;
            if (selectedCategory) {
                filteredAccounts = data.filter(account => account.categoryId === selectedCategory);
            }
            setAccounts(filteredAccounts);
        } catch (error) {
            console.error('Error fetching accounts:', error);
        }
    }, [selectedCategory, setAccounts]);

    const loadCategories = async () => {
        try {
            const data = await fetchCategories();
            setCategories(data);
        } catch (error) {
            console.error('Error fetching categories:', error);
        }
    };

    useEffect(() => {
        loadCategories();
        loadAccounts(); // Load accounts on initial render
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
                setSelectedAccountIds(prevSelected => {
                    const newSelected = new Set();
                    message.payload.forEach(account => {
                        if (prevSelected.has(account.id)) {
                            newSelected.add(account.id);
                        }
                    });
                    return newSelected;
                });
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, [loadAccounts]); // Added loadAccounts to dependency array

    const handleLoadAccounts = () => {
        loadAccounts();
    };

    const handleCategoryChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    useEffect(() => {
        loadAccounts(); // Reload accounts when category changes
    }, [selectedCategory, loadAccounts]); // Added loadAccounts to dependency array

    const handleSelectAccount = (accountId) => {
        setSelectedAccountIds(prevSelected => {
            const newSelected = new Set(prevSelected);
            if (newSelected.has(accountId)) newSelected.delete(accountId);
            else newSelected.add(accountId);
            return newSelected;
        });
    };

    const handleSelectAllAccounts = () => {
        if (selectedAccountIds.size === accounts.length && accounts.length > 0) {
            setSelectedAccountIds(new Set());
        } else {
            const allVisibleAccountIds = new Set(accounts.map(account => account.id));
            setSelectedAccountIds(allVisibleAccountIds);
        }
    };

    const handleStartSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để bắt đầu.');
            return;
        }

        if (selectedArticles.length === 0) {
            alert('Vui lòng chọn nội dung bài viết để bắt đầu.');
            return;
        }

        const config = {
            selectedAccountIds: Array.from(selectedAccountIds),
            selectedArticles: selectedArticles,
            concurrentThreads,
            maxPostsPerAccount,
            switchAccountOnError,
            switchAccountAfterPost,
            minPostInterval,
            maxPostInterval,
            commonConfig,
            postOrder,
            noDuplicatePosts,
            imageUploadWaitTime,
            includeBackgroundImage,
            commentAfterPost,
            commentContent
        };

        console.log('Starting posting with config:', config);
        csharpApi.startPosting(config);
        // Optionally, clear selection after starting
        // setSelectedAccountIds(new Set());
    };

    const handleStopSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để dừng.');
            return;
        }
        csharpApi.stopAccounts(Array.from(selectedAccountIds));
        // Optionally, clear selection after stopping
        // setSelectedAccountIds(new Set());
    };

    const accountCategories = categories.filter(c => c.type === 'account');

    // Handler for opening the ArticleManagement modal
    const handleOpenArticleManagement = () => {
        setIsArticleManagementOpen(true);
    };

    // Handler for closing the ArticleManagement modal
    const handleCloseArticleManagement = () => {
        setIsArticleManagementOpen(false);
    };

    // Handler for when articles are selected in ArticleManagement
    const handleArticlesSelected = (articles) => {
        setSelectedArticles(articles);
        setIsArticleManagementOpen(false); // Close the modal after selection
    };

    return (
        <div className="post-personal-container">
            <div className="config-panel">
                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">CẤU HÌNH ĐĂNG BÀI LÊN TRANG CÁ NHÂN</legend>
                    <button type="button" className="guideline-link">Xem hướng dẫn tại đây...</button>
                    
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="threads">⚙️</span> Số luồng chạy đồng thời <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input
                                type="number"
                                value={concurrentThreads}
                                onChange={(e) => setConcurrentThreads(parseInt(e.target.value))}
                                className="small-input"
                            />
                            <span>luồng</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="max-posts">📝</span> Mỗi tài khoản đăng tối đa</label>
                        <div className="input-group">
                            <input
                                type="number"
                                value={maxPostsPerAccount}
                                onChange={(e) => setMaxPostsPerAccount(parseInt(e.target.value))}
                                className="small-input"
                            />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="switch-error">🔄</span> Chuyển tài khoản nếu đăng lỗi <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input
                                type="number"
                                value={switchAccountOnError}
                                onChange={(e) => setSwitchAccountOnError(parseInt(e.target.value))}
                                className="small-input"
                            />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="switch-after-post">➡️</span> Chuyển tài khoản sau khi đăng</label>
                        <div className="input-group">
                            <input
                                type="number"
                                value={switchAccountAfterPost}
                                onChange={(e) => setSwitchAccountAfterPost(parseInt(e.target.value))}
                                className="small-input"
                            />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="interval">⏱️</span> Khoảng cách 2 lần đăng liên tiếp <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input
                                type="number"
                                value={minPostInterval}
                                onChange={(e) => setMinPostInterval(parseInt(e.target.value))}
                                className="range-input"
                            />
                            <span>-</span>
                            <input
                                type="number"
                                value={maxPostInterval}
                                onChange={(e) => setMaxPostInterval(parseInt(e.target.value))}
                                className="range-input"
                            />
                            <span>giây</span>
                        </div>
                    </div>
                </fieldset>

                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">Cấu hình bài viết</legend>
                    <div className="config-row">
                        <label className="switch">
                            <input
                                type="checkbox"
                                checked={commonConfig}
                                onChange={(e) => setCommonConfig(e.target.checked)}
                            />
                            <span className="slider round"></span>
                        </label>
                        <span>Cấu hình đăng chung cho tất cả các tài khoản</span>
                    </div>
                    <div className="content-selection">
                        <button className="btn btn-primary" onClick={handleOpenArticleManagement}><span role="img" aria-label="select-content">➕</span> Chọn nội dung</button>
                        <span>Tổng số nội dung chọn đăng: <strong>{selectedArticles.length}</strong></span>
                    </div>

                    <div className="radio-group config-row">
                        <label>
                            <input
                                type="radio"
                                name="post-order"
                                value="random"
                                checked={postOrder === 'random'}
                                onChange={(e) => setPostOrder(e.target.value)}
                            />
                            <span role="img" aria-label="random-post">🔀</span> Đăng ngẫu nhiên bài viết
                        </label>
                        <label>
                            <input
                                type="radio"
                                name="post-order"
                                value="sequential"
                                checked={postOrder === 'sequential'}
                                onChange={(e) => setPostOrder(e.target.value)}
                            />
                            <span role="img" aria-label="sequential-post">🔢</span> Đăng theo thứ tự
                        </label>
                    </div>

                    <div className="checkbox-group">
                        <label className="config-row">
                            <input
                                type="checkbox"
                                checked={noDuplicatePosts}
                                onChange={(e) => setNoDuplicatePosts(e.target.checked)}
                            />
                            <span role="img" aria-label="no-duplicate">🚫</span> Không đăng trùng bài viết giữa các tài khoản
                        </label>
                        <div className="config-row config-row-inline">
                            <label>
                                <input
                                    type="checkbox"
                                    checked={commentAfterPost ? true : imageUploadWaitTime > 0} // This checkbox is now tied to image upload wait time, assuming it's related
                                    onChange={(e) => {
                                        // If unchecking, set wait time to 0. If checking, set to default 1.
                                        setImageUploadWaitTime(e.target.checked ? 1 : 0);
                                    }}
                                />
                                <span role="img" aria-label="image-upload-time">⏳</span> Thời gian chờ tải ảnh lên
                            </label>
                            <input
                                type="number"
                                value={imageUploadWaitTime}
                                onChange={(e) => setImageUploadWaitTime(parseInt(e.target.value))}
                                className="small-input"
                                disabled={!imageUploadWaitTime} // Disable if the checkbox is unchecked (value is 0)
                            /> 
                            <span>giây</span>
                        </div>
                        <label className="config-row">
                            <input
                                type="checkbox"
                                checked={includeBackgroundImage}
                                onChange={(e) => setIncludeBackgroundImage(e.target.checked)}
                            />
                            <span role="img" aria-label="background-image">🖼️</span> Kèm ảnh background khi đăng trạng thái
                        </label>
                        <label className="config-row">
                            <input
                                type="checkbox"
                                checked={commentAfterPost}
                                onChange={(e) => setCommentAfterPost(e.target.checked)}
                            />
                            <span role="img" aria-label="comment-after-post">💬</span> Bình luận vào bài viết sau khi đăng thành công
                        </label>
                    </div>
                    <textarea
                        className="comment-guideline"
                        placeholder="Vui lòng nhập nội dung bình luận vào đây. Mỗi nội dung một dòng !"
                        value={commentContent}
                        onChange={(e) => setCommentContent(e.target.value)}
                        disabled={!commentAfterPost}
                    ></textarea>
                </fieldset>
            </div>

            <div className="account-panel">
                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">DANH SÁCH TÀI KHOẢN</legend>
                    <div className="toolbar">
                        <button className="icon-btn" onClick={handleLoadAccounts}>🔄</button>
                        <select
                            className="category-select"
                            value={selectedCategory}
                            onChange={handleCategoryChange}
                        >
                            <option value="">Chọn danh mục tài khoản</option>
                            {accountCategories.map(category => (
                                <option key={category.id} value={category.id}>{category.name}</option>
                            ))}
                        </select>
                        <button className="btn btn-load" onClick={handleLoadAccounts}>LOAD</button>
                        <div className="spacer"></div>
                        <button className="btn btn-start" onClick={handleStartSelected}>START</button>
                        <button className="btn btn-stop" onClick={handleStopSelected}>STOP</button>
                    </div>

                    <div className="table-wrapper">
                        {accounts.length > 0 ? (
                            <table>
                                <thead>
                                    <tr>
                                        <th>
                                            <input
                                                type="checkbox"
                                                onChange={handleSelectAllAccounts}
                                                checked={accounts.length > 0 && selectedAccountIds.size === accounts.length}
                                                ref={input => {
                                                    if (input) {
                                                        input.indeterminate = selectedAccountIds.size > 0 && selectedAccountIds.size < accounts.length;
                                                    }
                                                }}
                                            />
                                        </th>
                                        <th>STT</th>
                                        <th>UID</th>
                                        <th>Trạng thái</th>
                                        <th>Thành công</th>
                                        <th>Tình trạng</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {accounts.map((account, index) => (
                                        <tr key={account.id}>
                                            <td>
                                                <input
                                                    type="checkbox"
                                                    checked={selectedAccountIds.has(account.id)}
                                                    onChange={() => handleSelectAccount(account.id)}
                                                />
                                            </td>
                                            <td>{index + 1}</td>
                                            <td>{account.uid}</td>
                                            <td>{account.status}</td>
                                            <td>{account.successCount}</td>
                                            <td>{account.message}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <p>Không có tài khoản nào để hiển thị.</p>
                        )}
                    </div>
                </fieldset>
            </div>

            {/* Article Management Modal */}
            {isArticleManagementOpen && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <button className="modal-close-button" onClick={handleCloseArticleManagement}>X</button>
                        <ArticleManagement 
                            onClose={handleCloseArticleManagement} 
                            onSelectArticles={handleArticlesSelected} 
                            // Pass current selected articles to ArticleManagement if you want to pre-select them
                            initialSelectedArticles={selectedArticles}
                        />
                    </div>
                </div>
            )}
        </div>
    );
};

export default PostPersonal;
