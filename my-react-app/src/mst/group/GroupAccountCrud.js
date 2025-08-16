import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './GroupAccountCrud.css';

function GroupAccountCrud() {
    const [groupAccounts, setGroupAccounts] = useState([]);
    const [allGroupAccounts, setAllGroupAccounts] = useState([]); // Store all group accounts fetched
    const [pages, setPages] = useState([]); // Store pages to map groups to accounts
    const [accounts, setAccounts] = useState([]); // Store accounts to map categories
    const [categories, setCategories] = useState([]); // Store categories
    const [selectedCategory, setSelectedCategory] = useState(''); // State for selected category filter

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'groupAccountsData') {
                setGroupAccounts(message.payload);
                setAllGroupAccounts(message.payload); // Store all group accounts
            } else if (message.action === 'pageAccountsData') { // Assuming a new action for getting all page accounts
                setPages(message.payload);
            } else if (message.action === 'accountsData') {
                setAccounts(message.payload);
            } else if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'actionSuccess') {
                // Re-fetch all data after a successful action (e.g., deletion)
                csharpApi.getGroupAccounts();
                // You might need a new API call to get all page accounts if not already present
                // csharpApi.getPageAccounts(); 
                csharpApi.getAccounts();
                csharpApi.getCategories();
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getGroupAccounts();
        csharpApi.getAccounts(); // Fetch accounts to link with pages and categories
        csharpApi.getCategories(); // Fetch categories
        csharpApi.getPageAccounts(); // Fetch all page accounts

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    // Effect to filter group accounts based on selectedCategory
    useEffect(() => {
        if (selectedCategory && accounts.length > 0 && pages.length > 0 && categories.length > 0) {
            // 1. Find accounts belonging to the selected category
            const filteredAccountIds = accounts
                .filter(account => account.categoryId === selectedCategory)
                .map(account => account.id);
            
            // 2. Find pages belonging to the filtered accounts
            const filteredPageIds = pages
                .filter(page => filteredAccountIds.includes(page.accountId))
                .map(page => page.id);

            // 3. Filter group accounts based on the filtered pages
            const filteredGroups = allGroupAccounts.filter(group => 
                filteredPageIds.includes(group.pageId)
            );
            setGroupAccounts(filteredGroups);
        } else {
            setGroupAccounts(allGroupAccounts);
        }
    }, [selectedCategory, allGroupAccounts, pages, accounts, categories]);

    const handleDelete = (groupAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản nhóm ${groupAccount.name} này không?`)) {
            csharpApi.deleteGroupAccount(groupAccount);
        }
    };

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    const getPageName = (pageId) => {
        const page = pages.find(p => p.id === pageId);
        return page ? page.name : 'N/A';
    };

    const getAccountNameForGroup = (pageId) => {
        const page = pages.find(p => p.id === pageId);
        if (page && page.accountId) {
            const account = accounts.find(acc => acc.id === page.accountId);
            return account ? account.name : 'N/A';
        }
        return 'N/A';
    };

    const getAccountCategoryNameForGroup = (pageId) => {
        const page = pages.find(p => p.id === pageId);
        if (page && page.accountId) {
            const account = accounts.find(acc => acc.id === page.accountId);
            if (account && account.categoryId) {
                const category = categories.find(cat => cat.id === account.categoryId);
                return category ? category.name : 'N/A';
            }
        }
        return 'N/A';
    };

    const accountCategories = categories.filter(c => c.type === 'account');

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản Nhóm</h3>
                </div>
                <div className="filter-section">
                    <label htmlFor="categoryFilter">Lọc theo danh mục tài khoản:</label>
                    <select id="categoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
                        <option value="">Tất cả danh mục</option>
                        {accountCategories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên Nhóm</th>
                                <th>Group ID</th>
                                <th>Trang</th>
                                <th>Tài khoản</th>
                                <th>Danh mục Tài khoản</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {groupAccounts.map((groupAccount) => (
                                <tr key={groupAccount.id}>
                                    <td>{groupAccount.name}</td>
                                    <td>{groupAccount.groupId}</td>
                                    <td>{getPageName(groupAccount.pageId)}</td>
                                    <td>{getAccountNameForGroup(groupAccount.pageId)}</td>
                                    <td>{getAccountCategoryNameForGroup(groupAccount.pageId)}</td>
                                    <td className="actions-column">
                                        <button className="action-btn delete" onClick={() => handleDelete(groupAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default GroupAccountCrud;
