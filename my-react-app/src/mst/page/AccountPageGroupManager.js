import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './AccountPageGroupManager.css';
import '../account/AccountCrud.css'; // Reusing some account styles

function AccountPageGroupManager() {
    const [accounts, setAccounts] = useState([]);
    const [allAccounts, setAllAccounts] = useState([]); // Store all accounts fetched
    const [pages, setPages] = useState([]);
    const [groups, setGroups] = useState([]);
    const [categories, setCategories] = useState([]);

    const [selectedAccountId, setSelectedAccountId] = useState(null);
    const [selectedPageId, setSelectedPageId] = useState(null);
    const [selectedCategory, setSelectedCategory] = useState(''); // State for selected category filter

    const [loadingAccounts, setLoadingAccounts] = useState(false);
    const [loadingPages, setLoadingPages] = useState(false);
    const [loadingGroups, setLoadingGroups] = useState(false);

    // Fetch Accounts and Categories on initial load
    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
                setAllAccounts(message.payload); // Store all accounts
                setLoadingAccounts(false);
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'pagesByAccountIdData') {
                setPages(message.payload);
                setLoadingPages(false);
                setGroups([]); // Clear groups when pages for a new account are loaded
                setSelectedPageId(null); // Deselect page
            }
            if (message.action === 'groupsByPageIdData') {
                setGroups(message.payload);
                setLoadingGroups(false);
            }
            if (message.action === 'actionSuccess') {
                // Re-fetch data relevant to the action that just succeeded
                // For deletions, we need to refresh the current list
                if (selectedPageId) {
                    csharpApi.getGroupsByPageId(selectedPageId);
                } else if (selectedAccountId) {
                    csharpApi.getPagesByAccountId(selectedAccountId);
                } else {
                    csharpApi.getAccounts(); // Refresh accounts
                }
                csharpApi.getCategories(); // Refresh categories in case they changed
            }
        };

        csharpApi.addEventListener('message', handleMessage);

        setLoadingAccounts(true);
        csharpApi.getAccounts();
        csharpApi.getCategories();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, [selectedAccountId, selectedPageId]); // Keep dependencies to ensure fetches are tied to selections

    // Effect to filter accounts based on selectedCategory or display allAccounts
    useEffect(() => {
        if (selectedCategory) {
            const filtered = allAccounts.filter(account => account.categoryId === selectedCategory);
            setAccounts(filtered);
            // If the currently selected account is filtered out, clear selection
            if (selectedAccountId && !filtered.some(acc => acc.id === selectedAccountId)) {
                setSelectedAccountId(null);
                setSelectedPageId(null); // Also clear page selection
            }
        } else {
            setAccounts(allAccounts);
        }
    }, [selectedCategory, allAccounts, selectedAccountId]);


    // Fetch pages when selectedAccount changes
    useEffect(() => {
        if (selectedAccountId) {
            setLoadingPages(true);
            csharpApi.getPagesByAccountId(selectedAccountId);
        } else {
            setPages([]); // Clear pages if no account is selected
        }
    }, [selectedAccountId]);

    // Fetch groups when selectedPage changes
    useEffect(() => {
        if (selectedPageId) {
            setLoadingGroups(true);
            csharpApi.getGroupsByPageId(selectedPageId);
        } else {
            setGroups([]); // Clear groups if no page is selected
        }
    }, [selectedPageId]);

    const handleAccountClick = (account) => {
        setSelectedAccountId(account.id);
    };

    const handlePageClick = (page) => {
        setSelectedPageId(page.id);
    };

    const handleDeletePage = (pageAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản trang ${pageAccount.name} này không?`)) {
            csharpApi.deletePageAccount(pageAccount);
        }
    };

    const handleScanPage = (pageAccount) => {
        csharpApi.scanPageAccount(pageAccount);
    };

    const handleDeleteGroup = (groupAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản nhóm ${groupAccount.name} này không?`)) {
            csharpApi.deleteGroupAccount(groupAccount);
        }
    };

    const handleScanGroup = (groupAccount) => {
        csharpApi.scanGroupAccount(groupAccount);
    };

    const getCategoryName = (categoryId) => {
        const category = categories.find(c => c.id === categoryId);
        return category ? category.name : 'N/A';
    };

    const accountCategories = categories.filter(c => c.type === 'account');

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    return (
        <div className="account-page-group-manager-container">
            <div className="column-account">
                <div className="card">
                    <div className="card-header">
                        <h3>Tài khoản</h3>
                    </div>
                    <div className="filter-section">
                        <label htmlFor="accountCategoryFilter">Lọc theo danh mục tài khoản:</label>
                        <select id="accountCategoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
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
                                    <th>Tên</th>
                                    <th>UID</th>
                                    <th>Danh mục</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loadingAccounts ? (
                                    <tr><td colSpan="4">Đang tải tài khoản...</td></tr>
                                ) : accounts.length > 0 ? (
                                    accounts.map((account) => (
                                        <tr 
                                            key={account.id} 
                                            onClick={() => handleAccountClick(account)}
                                            className={selectedAccountId === account.id ? 'selected-row' : ''}
                                        >
                                            <td>{account.name}</td>
                                            <td>{account.uid}</td>
                                            <td>{getCategoryName(account.categoryId)}</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="4">Không có tài khoản nào.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div className="column-page-group">
                <div className="card">
                    <div className="card-header">
                        <h3>Trang của Tài khoản đã chọn</h3>
                    </div>
                    <div className="table-container">
                        <table className="crud-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Tên Trang</th>
                                    <th>Page ID</th>
                                    <th className="actions-column">Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {!selectedAccountId ? (
                                    <tr><td colSpan="4">Vui lòng chọn một tài khoản.</td></tr>
                                ) : loadingPages ? (
                                    <tr><td colSpan="4">Đang tải trang...</td></tr>
                                ) : pages.length > 0 ? (
                                    pages.map((page) => (
                                        <tr 
                                            key={page.id} 
                                            onClick={() => handlePageClick(page)}
                                            className={selectedPageId === page.id ? 'selected-row' : ''}
                                        >
                                            <td>{page.id}</td>
                                            <td>{page.name}</td>
                                            <td>{page.pageId}</td>
                                            <td className="actions-column">
                                                <button className="action-btn scan" onClick={(e) => {e.stopPropagation(); handleScanPage(page);}}>Quét</button>
                                                <button className="action-btn delete" onClick={(e) => {e.stopPropagation(); handleDeletePage(page);}}>Xóa</button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="4">Không có trang nào cho tài khoản này.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>

                <div className="card" style={{ marginTop: '20px' }}>
                    <div className="card-header">
                        <h3>Nhóm của Trang đã chọn</h3>
                    </div>
                    <div className="table-container">
                        <table className="crud-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Tên Nhóm</th>
                                    <th>Group ID</th>
                                    <th className="actions-column">Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {!selectedPageId ? (
                                    <tr><td colSpan="4">Vui lòng chọn một trang.</td></tr>
                                ) : loadingGroups ? (
                                    <tr><td colSpan="4">Đang tải nhóm...</td></tr>
                                ) : groups.length > 0 ? (
                                    groups.map((group) => (
                                        <tr key={group.id}>
                                            <td>{group.id}</td>
                                            <td>{group.name}</td>
                                            <td>{group.groupId}</td>
                                            <td className="actions-column">
                                                <button className="action-btn scan" onClick={(e) => {e.stopPropagation(); handleScanGroup(group);}}>Quét</button>
                                                <button className="action-btn delete" onClick={(e) => {e.stopPropagation(); handleDeleteGroup(group);}}>Xóa</button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="4">Không có nhóm nào cho trang này.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AccountPageGroupManager;
