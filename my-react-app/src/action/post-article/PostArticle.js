
import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './action/post-article/PostArticle.css'; // Using the revamped CSS

const PostArticle = () => {
    const [allCategories, setAllCategories] = useState([]);
    const [allAccounts, setAllAccounts] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [searchTerm, setSearchTerm] = useState('');
    const [generalConfig, setGeneralConfig] = useState({});
    const [contentConfig, setContentConfig] = useState({});

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'categoriesData') {
                setAllCategories(message.payload);
            }
            if (message.action === 'accountsData') {
                setAllAccounts(message.payload);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getCategories();
        csharpApi.getAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const accountCategories = allCategories.filter(c => c.Type === 'account');

    const filteredAccounts = allAccounts
        .filter(a => !selectedCategory || a.categoryId === parseInt(selectedCategory))
        .filter(a => a.name.toLowerCase().includes(searchTerm.toLowerCase()));

    const handleGeneralConfigChange = (e) => {
        setGeneralConfig({ ...generalConfig, [e.target.name]: e.target.value });
    };

    const handleContentConfigChange = (e) => {
        setContentConfig({ ...contentConfig, [e.target.name]: e.target.value });
    };

    return (
        <div className="post-article-container">
            <div className="main-content">
                <div className="card">
                    <div className="card-header">
                        <h3>Danh sách tài khoản</h3>
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
                            ))}
                        </select>
                        <input
                            type="text"
                            placeholder="Tìm kiếm tài khoản..."
                            className="account-search"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    
                    <div className="account-table-container">
                        <table className="accounts-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Tên</th>
                                    <th>Username</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredAccounts.map(account => (
                                    <tr key={account.id}>
                                        <td>{account.id}</td>
                                        <td>{account.name}</td>
                                        <td>{account.username}</td>
                                    </tr>
                                ))}
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
            </div>
        </div>
    );
};

export default PostArticle;
