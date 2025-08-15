
import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './PostArticle.css'; // Corrected import path

const PostArticle = () => {
    const [allCategories, setAllCategories] = useState([]);
    const [allAccounts, setAllAccounts] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [searchTerm, setSearchTerm] = useState('');
    const [generalConfig, setGeneralConfig] = useState({});
    const [contentConfig, setContentConfig] = useState({});
    const [selectedAccounts, setSelectedAccounts] = useState([]); // New state for selected accounts
    const [isPosting, setIsPosting] = useState(false); // New state to track if posting is active

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'categoriesData') {
                setAllCategories(message.payload);
            }
            if (message.action === 'accountsData') {
                setAllAccounts(message.payload);
            }
            if (message.action === 'postingStopped' || message.action === 'postingCompleted') {
                setIsPosting(false); // Stop posting state when stopped or completed
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getCategories();
        csharpApi.getAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    // Reset selected accounts when accounts or category filter changes
    useEffect(() => {
        setSelectedAccounts([]);
    }, [allAccounts, selectedCategory, searchTerm]);

    // Use c.type (camelCase) to filter categories
    const accountCategories = allCategories.filter(c => c.type === 'account');

    // Improved filtering for category and added UID search
    const filteredAccounts = allAccounts
        .filter(account => {
            const matchesCategory = !selectedCategory || 
                                    (account.categoryId && account.categoryId === selectedCategory);
            
            const lowerCaseSearchTerm = searchTerm.toLowerCase();
            const matchesSearchTerm = account.name.toLowerCase().includes(lowerCaseSearchTerm) ||
                                      (account.uid && account.uid.toLowerCase().includes(lowerCaseSearchTerm));
            
            return matchesCategory && matchesSearchTerm;
        });

    const handleGeneralConfigChange = (e) => {
        setGeneralConfig({ ...generalConfig, [e.target.name]: e.target.value });
    };

    const handleContentConfigChange = (e) => {
        setContentConfig({ ...contentConfig, [e.target.name]: e.target.value });
    };

    const handleAccountSelectChange = (accountId) => {
        setSelectedAccounts(prevSelected =>
            prevSelected.includes(accountId)
                ? prevSelected.filter(id => id !== accountId)
                : [...prevSelected, accountId]
        );
    };

    const handleSelectAllChange = (e) => {
        if (e.target.checked) {
            setSelectedAccounts(filteredAccounts.map(account => account.id));
        } else {
            setSelectedAccounts([]);
        }
    };

    const handleStartPosting = () => {
        if (selectedAccounts.length === 0) {
            alert("Vui lòng chọn ít nhất một tài khoản để đăng bài.");
            return;
        }

        const selectedAccountsData = allAccounts.filter(account => selectedAccounts.includes(account.id));
        
        const postConfig = {
            accounts: selectedAccountsData,
            general: generalConfig,
            content: contentConfig
        };

        csharpApi.postArticle(postConfig);
        setIsPosting(true); // Set posting state to true
        alert("Đã gửi yêu cầu đăng bài!");
    };

    const handleStopPosting = () => {
        csharpApi.cancelPostArticle(); // Call new C# API function to cancel
    };

    const isAllSelected = filteredAccounts.length > 0 && selectedAccounts.length === filteredAccounts.length;

    return (
        <div className="post-article-container">
            <div className="main-content">
                <div className="card">
                    <div className="card-header">
                        <h3>Danh sách tài khoản ({selectedAccounts.length} đã chọn)</h3> {/* Display selected count */}
                    </div>
                    <div className="account-selection-header">
                        <select 
                            value={selectedCategory} 
                            onChange={(e) => setSelectedCategory(e.target.value)}
                        >
                            <option value="">--Tất cả danh mục tài khoản--</option>
                            {accountCategories.map(category => (
                                <option key={category.id} value={category.id}> 
                                    {category.name}
                                </option>
                            ))}\
                        </select>
                        <input
                            type="text"
                            placeholder="Tìm kiếm theo tên hoặc UID..."
                            className="account-search"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    
                    <div className="account-table-container">
                        <table className="accounts-table">
                            <thead>
                                <tr>
                                    <th>
                                        <input
                                            type="checkbox"
                                            onChange={handleSelectAllChange}
                                            checked={isAllSelected}
                                        />
                                    </th>
                                    <th>Tên</th>
                                    <th>UID</th> 
                                </tr>
                            </thead>
                            <tbody>
                                {filteredAccounts.map(account => (
                                    <tr key={account.id}>
                                        <td>
                                            <input
                                                type="checkbox"
                                                checked={selectedAccounts.includes(account.id)}
                                                onChange={() => handleAccountSelectChange(account.id)}
                                            />
                                        </td>
                                        <td>{account.name}</td>
                                        <td>{account.uid}</td> 
                                    </tr>
                                ))}\
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div className="sidebar-content">
                <div className="card">
                    <div className="card-header">
                        <h3>Cấu hình chung</h3>
                    </div>
                    <form className="config-form">
                        <label htmlFor="setting1">Tên chiến dịch:</label>
                        <input id="setting1" name="campaignName" onChange={handleGeneralConfigChange} />
                        
                        <label htmlFor="setting2">Độ trễ giữa các bài đăng (giây):</label>
                        <input id="setting2" name="postDelay" type="number" onChange={handleGeneralConfigChange} />
                    </form>
                </div>
                <div className="card">
                    <div className="card-header">
                        <h3>Cấu hình nội dung</h3>
                    </div>
                    <form className="config-form">
                        <label htmlFor="content">Nội dung bài viết:</label>
                        <textarea id="content" name="content" rows="8" onChange={handleContentConfigChange}></textarea>
                    </form>
                </div>
                {isPosting ? (
                    <button className="stop-post-btn" onClick={handleStopPosting}>Dừng Đăng bài</button>
                ) : (
                    <button className="start-post-btn" onClick={handleStartPosting}>Bắt đầu Đăng bài</button>
                )}
            </div>
        </div>
    );
};

export default PostArticle;
