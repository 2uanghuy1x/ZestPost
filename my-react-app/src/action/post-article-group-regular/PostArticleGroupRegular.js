import React, { useState, useEffect } from 'react';
import AccountList from '../../components/AccountManagement/AccountList';
import '../../App.css'; 

const PostArticleGroupRegular = () => {
    const [articles, setArticles] = useState([]);
    const [selectedArticle, setSelectedArticle] = useState('');
    const [generalConfig, setGeneralConfig] = useState({
        postDelay: 5,
        repostInterval: 60,
    });

    useEffect(() => {
        // Fetch articles from API
        // This is a placeholder. Replace with actual API call.
        const fetchArticles = async () => {
            try {
                // Assuming api.js has a getArticles function
                // const response = await api.getArticles(); 
                // setArticles(response.data);
                setArticles([
                    { id: '1', title: 'Bài viết 1', content: 'Nội dung bài viết 1' },
                    { id: '2', title: 'Bài viết 2', content: 'Nội dung bài viết 2' },
                    { id: '3', title: 'Bài viết 3', content: 'Nội dung bài viết 3' },
                ]);
            } catch (error) {
                console.error("Error fetching articles:", error);
            }
        };
        fetchArticles();
    }, []);

    const handleGeneralConfigChange = (e) => {
        const { name, value } = e.target;
        setGeneralConfig(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleArticleChange = (e) => {
        setSelectedArticle(e.target.value);
    };

    const handleSubmit = () => {
        console.log("Selected Accounts:", "Need to get from AccountList");
        console.log("General Config:", generalConfig);
        console.log("Selected Article:", selectedArticle);
        // Implement post logic here
    };

    return (
        <div className="post-article-group-regular">
            <h2>Đăng bài nhóm thường</h2>

            <div className="section">
                <h3>Danh sách tài khoản</h3>
                <AccountList />
            </div>

            <div className="section">
                <h3>Cấu hình chung</h3>
                <div className="form-group">
                    <label>Độ trễ khi đăng (giây):</label>
                    <input 
                        type="number" 
                        name="postDelay" 
                        value={generalConfig.postDelay} 
                        onChange={handleGeneralConfigChange} 
                    />
                </div>
                <div className="form-group">
                    <label>Khoảng thời gian đăng lại (phút):</label>
                    <input 
                        type="number" 
                        name="repostInterval" 
                        value={generalConfig.repostInterval} 
                        onChange={handleGeneralConfigChange} 
                    />
                </div>
            </div>

            <div className="section">
                <h3>Cấu hình nội dung</h3>
                <div className="form-group">
                    <label>Chọn bài viết:</label>
                    <select value={selectedArticle} onChange={handleArticleChange}>
                        <option value="">-- Chọn bài viết --</option>
                        {articles.map(article => (
                            <option key={article.id} value={article.id}>
                                {article.title}
                            </option>
                        ))}
                    </select>
                </div>
                {selectedArticle && (
                    <div className="article-preview">
                        <h4>Nội dung bài viết đã chọn:</h4>
                        <p>{articles.find(art => art.id === selectedArticle)?.content}</p>
                    </div>
                )}
            </div>

            <button onClick={handleSubmit} className="submit-button">Bắt đầu đăng bài</button>
        </div>
    );
};

export default PostArticleGroupRegular;
