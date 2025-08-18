import React, { useState, useEffect } from 'react';
import AccountList from '../../components/AccountManagement/AccountList';
import '../../App.css'; 

const PostArticleGroupPage = () => {
    const [articles, setArticles] = useState([]);
    const [selectedArticle, setSelectedArticle] = useState('');
    const [generalConfig, setGeneralConfig] = useState({
        postDelay: 5,
        repostInterval: 60,
    });
    const [selectedPage, setSelectedPage] = useState('');
    const [pages, setPages] = useState([]); // State to hold pages

    useEffect(() => {
        // Fetch articles from API
        const fetchArticles = async () => {
            try {
                // const response = await api.getArticles(); 
                // setArticles(response.data);
                setArticles([
                    { id: '1', title: 'Bài viết A', content: 'Nội dung bài viết A' },
                    { id: '2', title: 'Bài viết B', content: 'Nội dung bài viết B' },
                    { id: '3', title: 'Bài viết C', content: 'Nội dung bài viết C' },
                ]);
            } catch (error) {
                console.error("Error fetching articles:", error);
            }
        };
        fetchArticles();

        // Fetch pages from API (placeholder)
        const fetchPages = async () => {
            try {
                // const response = await api.getPages();
                // setPages(response.data);
                setPages([
                    { id: 'page1', name: 'Page của tôi 1' },
                    { id: 'page2', name: 'Page của tôi 2' },
                ]);
            } catch (error) {
                console.error("Error fetching pages:", error);
            }
        };
        fetchPages();

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

    const handlePageChange = (e) => {
        setSelectedPage(e.target.value);
    };

    const handleSubmit = () => {
        console.log("Selected Accounts:", "Need to get from AccountList");
        console.log("General Config:", generalConfig);
        console.log("Selected Article:", selectedArticle);
        console.log("Selected Page:", selectedPage);
        // Implement post logic here
    };

    return (
        <div className="post-article-group-page">
            <h2>Đăng bài nhóm bằng Page</h2>

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
                    <label>Chọn Page để đăng:</label>
                    <select value={selectedPage} onChange={handlePageChange}>
                        <option value="">-- Chọn Page --</option>
                        {pages.map(page => (
                            <option key={page.id} value={page.id}>
                                {page.name}
                            </option>
                        ))}
                    </select>
                </div>
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

export default PostArticleGroupPage;
