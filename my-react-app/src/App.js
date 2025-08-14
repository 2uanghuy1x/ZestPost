
import React, { useState } from 'react';
import AccountCrud from './mst/account/AccountCrud';
import CategoryCrud from './mst/category/CategoryCrud';
import ArticleCrud from './mst/mst-article/ArticleCrud'; // Import the new ArticleCrud
import PostArticle from './action/post-article/PostArticle';
import Sidebar from './Sidebar';
import './App.css';

function App() {
    const [currentView, setCurrentView] = useState('accounts'); // Default view

    const handleNavigation = (view) => {
        setCurrentView(view);
    };

    let content;
    switch (currentView) {
        case 'accounts':
            content = <AccountCrud />;
            break;
        case 'categories':
            content = <CategoryCrud />;
            break;
        case 'articles': // Add the new case
            content = <ArticleCrud />;
            break;
        case 'post-article':
            content = <PostArticle />;
            break;
        default:
            content = <AccountCrud />;
    }

    return (
        <div className="app-container">
            <Sidebar onNavigate={handleNavigation} currentView={currentView} />
            <div className="main-app-content">
                {content}
            </div>
        </div>
    );
}

export default App;
