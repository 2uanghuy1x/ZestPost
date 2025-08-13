
import React, { useState, useEffect } from 'react';
import { csharpApi } from './api';
import './AccountCrud.css'; // We will create this CSS file next

function AccountCrud() {
    const [accounts, setAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentAccount, setCurrentAccount] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'actionSuccess') {
                csharpApi.getAccounts();
                setIsModalOpen(false);
                setCurrentAccount(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getAccounts();
        csharpApi.getCategories();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);
    
    const accountCategories = categories.filter(c => c.type === 'Account');

    const handleAddNew = () => {
        setCurrentAccount({ name: '', username: '', password: '', categoryId: '' });
        setIsModalOpen(true);
    };

    const handleEdit = (account) => {
        setCurrentAccount(account);
        setIsModalOpen(true);
    };

    const handleDelete = (account) => {
        if (window.confirm(`Are you sure you want to delete account ${account.name}?`)) {
            csharpApi.deleteAccount(account);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        const accountToSave = {
            ...currentAccount,
            categoryId: parseInt(currentAccount.categoryId, 10) || null
        };
        if (accountToSave.id) {
            csharpApi.updateAccount(accountToSave);
        } else {
            csharpApi.addAccount(accountToSave);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentAccount({ ...currentAccount, [name]: value });
    };

    const getCategoryName = (categoryId) => {
        const category = categories.find(c => c.id === categoryId);
        return category ? category.name : 'N/A';
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên</th>
                                <th>Username</th>
                                <th>Danh mục</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {accounts.map((account) => (
                                <tr key={account.id}>
                                    <td>{account.name}</td>
                                    <td>{account.username}</td>
                                    <td>{getCategoryName(account.categoryId)}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(account)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(account)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>{currentAccount && currentAccount.id ? 'Sửa tài khoản' : 'Thêm tài khoản'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <label>Tên:</label>
                            <input name="name" value={currentAccount?.name || ''} onChange={handleInputChange} required />
                            
                            <label>Username:</label>
                            <input name="username" value={currentAccount?.username || ''} onChange={handleInputChange} required/>

                            <label>Password:</label>
                            <input name="password" type="password" value={currentAccount?.password || ''} onChange={handleInputChange} />
                            
                            <label>Danh mục:</label>
                            <select name="categoryId" value={currentAccount?.categoryId || ''} onChange={handleInputChange}>
                                <option value="">-- Chọn danh mục --</option>
                                {accountCategories.map(cat => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>

                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AccountCrud;
